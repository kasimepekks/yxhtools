using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YXH_Tools_Files.Tools_CSV;
using YXH_Tools_Files.Tools_DirectoryandFile;
using YXH_Tools_Files.Tools_EdgeComputing;
using YXH_Tools_Files.Tools_Spike;

namespace YXH_Tools_Files.Tools_Excute
{
    public static class Tools_Excute_CSVSpikeRemoval
    {
        /// <summary>
        /// 该方法接受一个字符串列表作为输入数据，以及要替换的列号和新数据列表。
        /// 它将根据列号使用新数据替换原始数据，并返回修改后的原始数据。
        /// </summary>
        /// <param name="od">原数据</param>
        /// <param name="columntoreplace">需要替换的列号</param>
        /// <param name="nd">替换的数据</param>
        /// <exception cref="Exception">列号超出界限</exception>
        public static async Task ReplaceColumnInCSV(List<string> od, int columntoreplace, List<double> nd)
        {
           await Task.Run(() =>
            {
                //这里i要从1开始，因为od里的第0行是列名，不是数据
                for (int i = 1; i < od.Count; i++)
                {
                    // Split the line into an array of columns
                    string[] columns = od[i].Split(',');

                    // Check if the column index is within the range
                    if (columntoreplace >= 0 && columntoreplace < columns.Length)
                    {
                        // Replace the column with new data
                        columns[columntoreplace] = nd[i-1].ToString();
                    }
                    else
                    {
                        Console.WriteLine($"列号超出界限: {columntoreplace}");
                        throw new Exception($"列号超出界限: {columntoreplace}");
                    }
                    //替换原来的每一行string
                    od[i] = string.Join(",", columns);

                }
            });
            
           
        }

     
        /// <summary>
        /// 根据列号来删除特定列号数据的spike
        /// </summary>
        /// <param name="originalpath"></param>
        /// <param name="cleanpath"></param>
        /// <param name="configuredfolders"></param>
        /// <param name="columnstoreplace"></param>
        /// <returns></returns>
        public static async Task YXHExcuteCsvSpike(string originalpath, string cleanpath, string[] configuredfolders, List<int> columnstoreplace, int blockLengthMean, int blockLengthVariance, double threshold)
        {
            await Task.Run(async () =>  {
                Directory.CreateDirectory(cleanpath);
                //获得需要合并的源数据日期文件夹列表
                var ds = Tools_Excute_CSVCombine.YXHGetProcessedFolders(originalpath, cleanpath, configuredfolders);
                if (ds != null)
                {
                    foreach (var d in ds)
                    {
                        Stopwatch timer = new Stopwatch();
                        timer.Start();
                        Console.WriteLine("开始删除spike文件夹名为" + d.Name + "的文件，请稍等片刻");
                        var fileinfo = BasicFilesOperation.YXHGetAllextFiles(d.FullName, "*.csv");

                        if (fileinfo.Length == 0 || fileinfo == null)
                        {
                            Console.WriteLine("文件路径可能有错误，无法读取到csv文件！！！");
                        }
                        else
                        {
                            //筛选0kb的csv的数据并转换成txt
                            fileinfo.YXHTransferZeroCsvtoTxt();
                            for (int i = 0; i < fileinfo.Length; i++)
                            {
                                //文件大小不等0则需要clean
                                if (fileinfo[i].Length != 0)
                                {
                                    string resultdirectorypath = Path.Combine(cleanpath, d.Name);
                                    DirectoryInfo root = new DirectoryInfo(resultdirectorypath);
                                    if (!root.Exists)
                                    {
                                        root.Create();
                                    }
                                    string resultpath = Path.Combine(root.FullName, fileinfo[i].Name);
                                    var odlist =await  CSVOperator.YXHReadCSV2ListOneTime(fileinfo[i]);//读取csv数据到List<string>,每一行为一个string
                                    //读取csv多个特定列的数据到List<List<double>>>，这是需要去除spike的数据集
                                    var datalists =await CSVOperator.YXHReadColumnsfromCSV2List(fileinfo[i].FullName,columnstoreplace);

                                    for (int j = 0; j < columnstoreplace.Count; j++)
                                    {
                                        //去除spike后的数据
                                       var spikeremovelist= RemoveSpikeProcess.completeSpikeRemovalProcess(datalists[j], blockLengthMean, blockLengthVariance, threshold); 
                                      //把去除spike后的数据替换到原数据
                                       await ReplaceColumnInCSV(odlist, columnstoreplace[j], spikeremovelist);
                                    }
                                    //最后输出CSV                                  
                                    await CSVOperator.YXHSaveCSVfromListOneTime(resultpath, odlist);
                                }

                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("合并路径有问题，请检查配置文件");
                }



            });
        }
    }
}
