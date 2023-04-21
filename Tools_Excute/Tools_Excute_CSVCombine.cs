using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YXH_Tools_Files.Tools_CSV;
using YXH_Tools_Files.Tools_DirectoryandFile;
using YXH_Tools_Files.Tools_EdgeComputing;

namespace YXH_Tools_Files.Tools_Excute
{
    /// <summary>
    /// 执行合并操作类
    /// </summary>
    public static class Tools_Excute_CSVCombine
    {
        /// <summary>
        /// （专用于边缘采集器）根据combinetype来执行合并操作，有2种选择，一个是按照30分钟进行合并，另一个是按照txt来合并
        /// </summary>
        /// <param name="rootpath">需要合并的文件夹路径</param>
        /// <param name="combinedpath">合并后的目标文件夹路径</param>
        /// <param name="filelist">指定需要合并的日期文件夹，如为null，则表明需要合并所有的rootpath下的子文件夹，如不为null，则表明需要合并指定的日期文件夹</param>
        /// <param name="combinetype">合并方式：1为按时间合并，0为按txt文件合并，为2则为里程合并</param>
        public static void YXHExcuteCombination(string rootpath, string combinedpath, string[] filelist, string combinetype,int permileage, int spdindex)
        {
            try
            {
                //获取input路径所有的子文件夹
                var subdirectorieslist = BasicFilesOperation.YXHGetSubDirectories(rootpath);
                //获取目标路径所有的子文件夹
                var subtargetdirectorieslist = BasicFilesOperation.YXHGetSubDirectories(combinedpath);
                //获取目标路径所有的子文件夹名
                var targetlist = subtargetdirectorieslist.Select(s => s.Name).ToArray();
                DirectoryInfo[] specialdirectory;
                if (filelist != null)
                {
                    specialdirectory = subdirectorieslist.Where(s => filelist.Contains(s.Name)).ToArray();
                }
                else
                {
                    specialdirectory = subdirectorieslist;
                }
                if (targetlist.Length > 0)
                {
                    //选择目标路径里没有的文件夹，如有说明之前可能已经合并过了，不需要合并，减少合并时间
                    specialdirectory = specialdirectory.Where(s => !targetlist.Contains(s.Name)).ToArray();
                    foreach (var s in targetlist)
                    {
                        Console.WriteLine("目标路径已有" + s + "的文件夹，不合并此日期文件！");
                    }
                }
                Stopwatch timer = new Stopwatch();
                //ParallelOptions options = new ParallelOptions();
                //options.MaxDegreeOfParallelism = 10;
                timer.Start();
                //创建多线程list
                //List<Task>t_out=new List<Task>();
                ParallelOptions options = new ParallelOptions();
                options.MaxDegreeOfParallelism = 3;//支持3个并发执行,控制数量
                Parallel.ForEach(specialdirectory, options, d => {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    Console.WriteLine("开始合并文件夹名为" + d.Name + "的文件");
                    //List<Task> t_in = new List<Task>();
                    //获得子文件日期名，如"07_01"
                    string filedate = d.Name;
                    //获取合并后的指定的日期文件夹路径
                    string finalcombinedpath = Path.Combine(combinedpath, filedate);
                    List<List<FileInfo>> classifylist = new List<List<FileInfo>>();
                    //筛选0kb的csv的数据并转换成txt
                    BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.csv").YXHTransferZeroCsvtoTxt();
                    //按时间合并
                    if (combinetype == "1")
                    {
                        //获取每一个子文件夹的所有csv文件，并按文件名排序
                        var allfiles = BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.csv");

                        //每半个小时分类汇总成一个总classifylist，每个list里有各自相同时间段的数据
                        classifylist = FileInfoOperation_Combination.YXHClassifyFilebyTime(allfiles);
                    }
                    //按里程来分布
                    else if(combinetype == "2")
                    {
                        //获取每一个子文件夹的所有csv文件，并按文件名排序
                        var allfiles = BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.csv");
                        //每多少公里分类汇总成一个总classifylist，每个list里有各自相同里程段的数据
                        classifylist = FileInfoOperation_Combination.YXHClassifyFilebyMileage(allfiles,permileage,spdindex);
                    }
                    else
                    {
                        //获取每一个子文件夹的所有txt和csv文件，并按文件名排序
                        var allfiles = BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.*").Where(s => s.Name.ToLower().EndsWith(".csv") || s.Name.ToLower().EndsWith(".txt")).ToArray();
                        //每半个小时分类汇总成一个总classifylist，每个list里有各自相同时间段的数据
                        classifylist = FileInfoOperation_Combination.YXHClassifyFilebyTxt(allfiles);
                    }
                    foreach (var c in classifylist)
                    {
                        if (c.Count > 0)
                        {
                            string combinefilepath;
                            if (combinetype == "1")
                            {
                                combinefilepath = Path.Combine(finalcombinedpath, c[0].YXHGetCombinedFilename());
                            }
                            else
                            {
                                if (c.Count > 1)
                                {
                                    combinefilepath = Path.Combine(finalcombinedpath, c[0].YXHGetCombinedFilenamebyTxt(c[c.Count - 1]));
                                }
                                else
                                {
                                    combinefilepath = Path.Combine(finalcombinedpath, c[0].Name);
                                }
                            }
                            //处理每个classifylist里的csv文件
                            for (int i = 0; i < c.Count; i++)
                            {
                                var csvlist = CSVOperator.YXHCSV2List(c[i].FullName, i);
                                CSVOperator.YXHList2CSV(csvlist, combinefilepath);
                            }

                            Console.WriteLine(d.Name + "合并中，请耐心等待……" + c.Count);

                        }
                    }
                    if (true)
                    {
                        FileInfoOperation_Combination.YXHAddTxt(finalcombinedpath);
                    }
                    timer.Stop();
                    Console.WriteLine(filedate + "合并完成时间为" + timer.Elapsed.TotalMinutes.ToString("0.00") + "分");
                });
                timer.Stop();
                Console.WriteLine("完成总时间为" + timer.Elapsed.TotalMinutes.ToString("0.00") + "分");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }

        /// <summary>
        ///（专用于边缘采集器） 获得需要处理的日期文件夹列表
        /// </summary>
        /// <param name="originalpath">源数据路径</param>
        /// <param name="combinedpath">指定的合并后的路径</param>
        /// <param name="configuredfolders">指定需要合并的日期文件夹，这是在配置文件读取到的日期数据</param>
        /// <returns></returns>
        public static DirectoryInfo[] YXHGetProcessedFolders(string originalpath, string combinedpath, string[] configuredfolders)
        {
            //获取input路径所有的子文件夹
            var subdirectorieslist = BasicFilesOperation.YXHGetSubDirectories(originalpath);
            //获取目标路径所有的子文件夹
            var subtargetdirectorieslist = BasicFilesOperation.YXHGetSubDirectories(combinedpath);
            //获取指定合并路径下所有已经存在的子文件夹名
            var targetlist = subtargetdirectorieslist.Select(s => s.Name).ToArray();
            DirectoryInfo[] specialdirectory;
            //如为null，则表明需要合并所有的originalpath下的子文件夹，如不为null，则表明需要合并指定的日期文件夹
            if (configuredfolders != null)
            {
                specialdirectory = subdirectorieslist.Where(s => configuredfolders.Contains(s.Name)).ToArray();
            }
            else
            {
                specialdirectory = subdirectorieslist;
            }
            if (targetlist.Length > 0)
            {
                //选择目标路径里没有的文件夹，如有说明之前可能已经合并过了，不需要合并，减少合并时间
                specialdirectory = specialdirectory.Where(s => !targetlist.Contains(s.Name)).ToArray();
                foreach (var s in targetlist)
                {
                    Console.WriteLine("目标路径已有" + s + "的文件夹，不合并此日期文件！");
                }
            }
            return specialdirectory;
        }
        /// <summary>
        /// （专用于边缘采集器）根据合并类型来获得不同的分类好的文件列表
        /// </summary>
        /// <param name="d">需要分类的文件夹</param>
        /// <param name="combinetype">合并方式：1为按时间合并，0为按txt文件合并，为2则为里程合并</param>
        /// <param name="permileage">里程合并的基准里程</param>
        /// <param name="spdindex">里程合并的指定csv里的车速index</param>
        public static List<List<FileInfo>> YXHGetClassifyFileListperFolder(DirectoryInfo d,string combinetype, int permileage, int spdindex)
        {
            List<List<FileInfo>> classifylist = new List<List<FileInfo>>();
            //筛选0kb的csv的数据并转换成txt
            BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.csv").YXHTransferZeroCsvtoTxt();
            //按时间合并
            if (combinetype == "1")
            {
                //获取每一个子文件夹的所有csv文件，并按文件名排序
                var allfiles = BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.csv");

                //每半个小时分类汇总成一个总classifylist，每个list里有各自相同时间段的数据
                classifylist = FileInfoOperation_Combination.YXHClassifyFilebyTime(allfiles);
            }
            //按里程来分布
            else if (combinetype == "2")
            {
                //获取每一个子文件夹的所有csv文件，并按文件名排序
                var allfiles = BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.csv");
                //每多少公里分类汇总成一个总classifylist，每个list里有各自相同里程段的数据
                classifylist = FileInfoOperation_Combination.YXHClassifyFilebyMileage(allfiles, permileage, spdindex);
            }
            else
            {
                //获取每一个子文件夹的所有txt和csv文件，并按文件名排序
                var allfiles = BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.*").Where(s => s.Name.ToLower().EndsWith(".csv") || s.Name.ToLower().EndsWith(".txt")).ToArray();
                //每半个小时分类汇总成一个总classifylist，每个list里有各自相同时间段的数据
                classifylist = FileInfoOperation_Combination.YXHClassifyFilebyTxt(allfiles);
            }
            return classifylist;
        }
        /// <summary>
        /// （专用于边缘采集器）获得每个分类list合并后的文件路径
        /// </summary>
        /// <param name="d"></param>
        /// <param name="filelist"></param>
        /// <param name="combinedpath"></param>
        /// <param name="combinetype"></param>
        /// <returns></returns>
        public static string YXHGetCombinedFileName(DirectoryInfo d, List<FileInfo> filelist,string combinedpath, string combinetype)
        {
            try
            {
                string filedate = d.Name;
                string combinefilepath = "";
                //获取合并后的指定的日期文件夹路径
                string finalcombinedpath = Path.Combine(combinedpath, filedate);
                if (filelist.Count > 0)
                {

                    //时间模式
                    if (combinetype == "1")
                    {
                        combinefilepath = Path.Combine(finalcombinedpath, filelist[0].YXHGetCombinedFilename());
                    }
                    else
                    {
                        if (filelist.Count > 1)
                        {
                            combinefilepath = Path.Combine(finalcombinedpath, filelist[0].YXHGetCombinedFilenamebyTxt(filelist[filelist.Count - 1]));
                        }
                        else
                        {
                            combinefilepath = Path.Combine(finalcombinedpath, filelist[0].Name);
                        }
                    }

                }
                return combinefilepath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
          
        }
        /// <summary>
        /// （专用于边缘采集器）执行一个分类好的filelist的合并动作，并输出到指定的csv文件
        /// </summary>
        /// <param name="filelist">一个分类好的filelist</param>
        /// <param name="outputpath">输出到指定的csv文件路径</param>
        public static void YXHExcuteCombinationPerClassify(List<FileInfo> filelist, string outputpath)
        {
            try
            {
                if (outputpath != "")
                {
                    for (int i = 0; i < filelist.Count; i++)
                    {
                        var csvlist = CSVOperator.YXHCSV2List(filelist[i].FullName, i);
                        CSVOperator.YXHList2CSV(csvlist, outputpath);
                    }
                }
                else
                {
                    Console.WriteLine("输出文件路径为空，请检查！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); ;
            }
        
        }

        /// <summary>
        /// （专用于边缘采集器）根据combinetype来执行合并操作
        /// </summary>
        /// <param name="originalpath"></param>
        /// <param name="combinedpath"></param>
        /// <param name="configuredfolders"></param>
        /// <param name="combinetype"></param>
        /// <param name="permileage"></param>
        /// <param name="spdindex"></param>
        public static void YXHExcuteCombinationAll(string originalpath, string combinedpath, string[] configuredfolders, string combinetype, int permileage, int spdindex,int threadsnum)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            //先创建输出文件夹
            Directory.CreateDirectory(combinedpath);
            //获得需要合并的源数据日期文件夹列表
            var ds=YXHGetProcessedFolders(originalpath, combinedpath, configuredfolders);
            foreach (var d in ds)
            {
                Console.WriteLine("开始合并文件夹名为" + d.Name + "的文件，请稍等片刻");
              
                //获得分类好的文件列表
                var classifiedlist=YXHGetClassifyFileListperFolder(d,combinetype,permileage, spdindex);
                Console.WriteLine("已完成分类文件列表");


                //启动多线程
                ParallelOptions options = new ParallelOptions();
                options.MaxDegreeOfParallelism = threadsnum;//支持threadsnum个并发执行,控制数量
                Parallel.ForEach(classifiedlist, options, c =>
                {
                    //获得输出的文件路径
                    var outputpath = YXHGetCombinedFileName(d, c, combinedpath, combinetype);
                    //执行合并csv动作
                    YXHExcuteCombinationPerClassify(c, outputpath);

                });
                timer.Stop();
                //获得输出日期文件夹路径
                var dateoutputpath= Path.Combine(combinedpath, d.Name);
                //添加done.txt文件
                FileInfoOperation_Combination.YXHAddTxt(dateoutputpath);
                Console.WriteLine("完成此次操作工花费了"+timer.Elapsed.TotalMinutes.ToString("0.00") + "分");
            }
        }
    }
}
