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
        /// <param name="combinetype">合并方式：1为按时间合并，0为按txt文件合并</param>
        public static void YXHExcuteCombination(string rootpath, string combinedpath, string[] filelist, string combinetype)
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
                Parallel.ForEach(specialdirectory, d => {
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
                    if (combinetype == "1")
                    {
                        //获取每一个子文件夹的所有csv文件，并按文件名排序
                        var allfiles = BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.csv");

                        //每半个小时分类汇总成一个总classifylist，每个list里有各自相同时间段的数据
                        classifylist = FileInfoOperation_Combination.YXHClassifyFilebyTime(allfiles);
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

                            for (int i = 0; i < c.Count; i++)
                            {
                                var csvlist = CSVOperator.YXHCSV2List(c[i].FullName, i);
                                CSVOperator.YXHList2CSV(csvlist, combinefilepath);
                            }

                            Console.WriteLine(d.Name + "合并中，请耐心等待……" + c.Count);

                        }
                    }
                    if (combinetype == "1")
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
    }
}
