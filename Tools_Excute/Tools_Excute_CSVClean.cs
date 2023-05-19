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

namespace YXH_Tools_Files.Tools_Excute
{
    public static class Tools_Excute_CSVClean
    {
        public static async Task YXHExcuteCsvClean(string originalpath, string cleanpath, string[] configuredfolders,string renamecollumn, string[] cleannumreg, string[] cleanstringreg)
        {
            try
            {
                //先创建输出文件夹
                Directory.CreateDirectory(cleanpath);
                //获得需要合并的源数据日期文件夹列表
                var ds = Tools_Excute_CSVCombine.YXHGetProcessedFolders(originalpath, cleanpath, configuredfolders);
                if (ds != null)
                {
                    foreach (var d in ds)
                    {
                        Stopwatch timer = new Stopwatch();
                        timer.Start();
                        Console.WriteLine("开始清洗文件夹名为" + d.Name + "的文件，请稍等片刻");
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
                                if(fileinfo[i].Length != 0)
                                {
                                    var result = await CSVOperator.YXHReadCSV2ListOneTime(fileinfo[i]);//读取csv数据到List<string>,每一行为一个string
                                    if(result.Count>0)
                                    {
                                        //如果填的是same，就不需要改名
                                        if (renamecollumn != "same")
                                        {
                                            //判断配置文件里的字段名称的长度是否等于csv里的字段名称的长度
                                            if (result[0].Split(",").Length == renamecollumn.Replace("，", ",").Split(",").Length)
                                            {
                                                //把配置文件里的字段名称替换到csv里的字段名称
                                                result[0] = renamecollumn.Replace("，", ",");

                                            }
                                           
                                           
                                        }
                                        string resultdirectorypath = Path.Combine(cleanpath, d.Name);
                                        DirectoryInfo root = new DirectoryInfo(resultdirectorypath);
                                        if (!root.Exists)
                                        {
                                            root.Create();
                                        }
                                        string resultpath = Path.Combine(root.FullName, fileinfo[i].Name);
                                        for (int j = 0; j < result.Count; j++)
                                        {
                                            if (cleannumreg.Length > 0)
                                            {
                                                foreach (var r in cleannumreg)
                                                {
                                                    result[j] = Regex.Replace(result[j], @"(?<![\d.-])\" + r + @"(?![\d.-])", "0");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("清洗规则配置错误，请查看");
                                                throw new Exception("程序错误：清洗规则配置错误，请查看");
                                            }
                                            if (cleanstringreg.Length > 0)
                                            {
                                                foreach (var r in cleanstringreg)
                                                {
                                                    //把字符串类型的规则也清零
                                                    result[j] = result[j].Replace(r,"0");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("清洗规则配置错误，请查看");
                                                throw new Exception("程序错误：清洗规则配置错误，请查看");
                                            }
                                            //result[j]=result[j].Replace("nan", "0");
                                            //result[j]= Regex.Replace(result[j], @"(?<![\d.-])\-1(?![\d.-])", "0");//把-1转成0
                                            //result[j] = Regex.Replace(result[j], @"(?<![\d.-])\-2(?![\d.-])", "0");//把-2转成0
                                            //result[j] = Regex.Replace(result[j], @"(?<![\d.-])\-3(?![\d.-])", "0");//把-3转成0
                                            //result[j] = Regex.Replace(result[j], @"(?<![\d.-])\-100(?![\d.-])", "0");//把-100转成0
                                        }
                                        await CSVOperator.YXHSaveCSVfromListOneTime(resultpath, result);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("合并路径有问题，请检查配置文件");
                }
            }

            catch (Exception ex)
            {

                Console.WriteLine(ex.Message); ;
            }

        }

    }
}
