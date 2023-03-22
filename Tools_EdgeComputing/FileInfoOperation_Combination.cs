using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.Tools_EdgeComputing
{
    /// <summary>
    ///  FileInfo文件合并操作类，包括合并后文件名，合并规则等
    /// </summary>
    public static class FileInfoOperation_Combination
    {
        /// <summary>
        /// （专用于边缘采集器）生成合并好的csv文件名
        /// </summary>
        /// <param name="file">FileInfo类型文件</param>
        /// <returns>string合并后文件名</returns>
        public static string YXHGetCombinedFilename(this FileInfo file)
        {
            string date = file.Name.Split("-")[0];
            string hour = file.Name.Split("-")[1].Split("_")[0];
            string minute = file.Name.Split("-")[1].Split("_")[1];
            if (Convert.ToInt16(minute) < 30)
            {
                return date + "-" + hour + "_" + "01" + "_00-F_1.csv";
            }
            else
            {
                return date + "-" + hour + "_" + "31" + "_00-F_1.csv";
            }
        }
        /// <summary>
        ///（专用于边缘采集器） 返回一组以30分钟来分割的时间线，如（2022-07-11 00:00:00），一共有24个
        /// </summary>
        /// <param name="file">FileInfo类型文件</param>
        /// <returns>List<DateTime></returns>
        public static List<DateTime> YXHGetSplitTime(this FileInfo file)
        {
            List<DateTime> list = new List<DateTime>();
            string date = file.YXHFilenametoDate();
            for (int i = 0; i < 24; i++)
            {
                list.Add(Convert.ToDateTime(date + " " + i.ToString("D2") + ":00:00"));
                list.Add(Convert.ToDateTime(date + " " + i.ToString("D2") + ":30:00"));
            }
            list.Add(Convert.ToDateTime(date + " " + "23:59:59"));
            return list;
        }
        /// <summary>
        /// （专用于边缘采集器）按照时间顺序来分类并返回分类好的CSV文件组
        /// </summary>
        /// <param name="files">FileInfo类型文件组</param>
        /// <returns>List<List<FileInfo>></returns>
        public static List<List<FileInfo>> YXHClassifyFilebyTime(this FileInfo[] files)
        {
            //创建一个totallist，用于存储每半个小时的list
            List<List<FileInfo>> totallist = new List<List<FileInfo>>();
            //创建48个List<FileInfo>()，并添加到totallist
            for (int i = 0; i < 49; i++)
            {
                totallist.Add(new List<FileInfo>());
            }
            if (files.Length > 0)
            {
                //用第一个文件来获取48个分割时间线，因为要用到第一个文件的文件名
                List<DateTime> timelist = files[0].YXHGetSplitTime();
                foreach (var file in files)
                {
                    for (int i = 0; i < timelist.Count; i++)
                    {
                        if (file.YXHFilenametoDateTime() >= timelist[i] && file.YXHFilenametoDateTime() < timelist[i + 1])
                        {
                            totallist[i].Add(file);
                        }
                    }
                }
            }
            return totallist;
        }
        /// <summary>
        /// （专用于边缘采集器）查找0kb的数据，并把它替换成txt文件
        /// </summary>
        /// <param name="fileInfos">文件</param>
        public static void YXHTransferZeroCsvtoTxt(this FileInfo[] fileInfos)
        {
            if (fileInfos.Length > 0)
            {
                foreach (var item in fileInfos)
                {
                    if (item.Length == 0)
                    {
                        var txt = item.FullName.Replace("csv", "txt");
                        File.Move(item.FullName, txt);
                    }
                }
            }
        }
        /// <summary>
        ///（专用于边缘采集器）按照TXT来分类并返回分类好的CSV文件组
        /// </summary>
        /// <param name="allfile">FileInfo类型文件组</param>
        /// <returns></returns>
        public static List<List<FileInfo>> YXHClassifyFilebyTxt(FileInfo[] allfile)
        {
            //创建一个totallist，用于存储每2个txt中间的数据的list
            List<List<FileInfo>> totallist = new List<List<FileInfo>>();
            List<int> txtindex = new List<int>();
            for (int i = 0; i < allfile.Length; i++)
            {
                if (allfile[i].Extension.ToLower() == ".txt")
                {
                    txtindex.Add(i);
                }
            }
            //txt有1个以上
            if (txtindex.Count > 0)
            {
                //添加第一个list，用于存放第一个txt之前的csv数据，不管有没有csv
                totallist.Add(new List<FileInfo>());
                for (int j = 0; j < txtindex[0]; j++)
                {
                    totallist[0].Add(allfile[j]);
                }
                for (int i = 0; i < txtindex.Count - 1; i++)
                {
                    totallist.Add(new List<FileInfo>());
                    //如果第一个数的后一个数比第二个数前一个数小，就说明中间有csv需要合并
                    if (txtindex[i] + 1 <= txtindex[i + 1] - 1)
                    {
                        for (int j = txtindex[i] + 1; j <= txtindex[i + 1] - 1; j++)
                        {
                            totallist[i+1].Add(allfile[j]);//这里totallist[i+1]是因为totallist[0]已经有了，必须从1开始
                        }
                    }
                }
                //添加最后一个list，用于存放最后一个txt之后的csv数据，不管有没有csv
                totallist.Add(new List<FileInfo>());
                for (int j = txtindex[txtindex.Count - 1] + 1; j < allfile.Length; j++)
                {
                    totallist[totallist.Count - 1].Add(allfile[j]);
                }
            }
            //如果没有txt，那就直接把所有的csv文件放在totallist[0]里进行合并
            else
            {
                totallist.Add(new List<FileInfo>());
                for (int j = 0; j < allfile.Length; j++)
                {
                    totallist[0].Add(allfile[j]);
                }
            }
            return totallist;
        }
        /// <summary>
        /// （专用于边缘采集器）按照txt来合并csv后的csv文件名
        /// </summary>
        /// <param name="file1">List里第一个file</param>
        /// <param name="file2">List里最后一个file</param>
        /// <returns>string合并后的文件名</returns>
        public static string YXHGetCombinedFilenamebyTxt(this FileInfo file1, FileInfo file2)
        {
            var file1name = file1.Name.Split(".")[0];
            var file2name = file2.Name.Split(".")[0];
            return file1name + "-to-" + file2name + ".csv";
        }
        /// <summary>
        /// （专用于边缘采集器）在指定文件夹下创建done.txt文件
        /// </summary>
        /// <param name="directorypath">日期文件夹路径</param>
        public static void YXHAddTxt( string directorypath)
        {
            string txtname = directorypath.Split('\\')[directorypath.Length - 1]+ "_done.txt";
            string txtpath =Path.Combine(directorypath,txtname);
            FileStream fs = new FileStream(txtpath, FileMode.Append);
            StreamWriter? wr = null;
            wr = new StreamWriter(fs);
            wr.Close();
        }
    }
}
