using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using YXH_Tools_Files.Tools_CSV;
using YXH_Tools_Files.Tools_DirectoryandFile;
using YXH_Tools_Files.Tools_List;

namespace YXH_Tools_Files.Tools_Excute
{
    /// <summary>
    /// 执行ncode的csv表头编辑操作类
    /// </summary>
    public static  class Tools_Excute_HeaderEdit_for_NcodeCSV
    {
        /// <summary>
        /// 执行ncode的csv表头编辑操作，结果与边缘采集器的csv文件表头一致
        /// </summary>
        /// <param name="directorypath">输入文件夹</param>
        /// <param name="outputpath">输出文件夹</param>
        /// <returns></returns>
    
        public static async Task YXHExcuteHeaderAsync(string directorypath,string outputpath)
        {
            try
            {
                var fileinfo = BasicFilesOperation.YXHGetAllextFiles(directorypath, "*.csv");
                if (fileinfo.Length == 0 || fileinfo == null)
                {
                    Console.WriteLine("文件路径可能有错误，无法读取到csv文件！！！");
                }
                else
                {
                    for (int i = 0; i < fileinfo.Length; i++)
                    {
                        Console.WriteLine($"********************开始编辑第{i}个文件{fileinfo[i].Name} - {DateTime.Now}********************");
                        var result = await CSVOperator.YXHReadCSV2ListOneTime(fileinfo[i]);//读取csv数据到List<string>,每一行为一个string
                        ListOperation.YXHEditCsvTitle(ref result);
                        ListOperation.YXHEditCsvDataSP(ref result);
                        string newname;
                        newname = fileinfo[i].Name.Split(".")[0] + "_headeredit.csv";
                        string resultpath = Path.Combine(outputpath, newname);
                        await CSVOperator.YXHSaveCSVfromListOneTime(resultpath, result);
                        Console.WriteLine($"********************第{i}个文件编辑完成 - {DateTime.Now}********************");
                    }
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }
    }
}
