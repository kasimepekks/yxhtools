using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YXH_Tools_Files.ObjectClass;

namespace YXH_Tools_Files.Tools_CSV
{
    public static class CSVOperator
    {
        private const double internaltime = 0.001953125;//此属性是1/512的数值，采集一个点的时间间隔
        /// <summary>
        /// 用于把内存中的datatable输出到指定目录下的csv文件的方式1，如目录不存在则会创建目录。
        /// </summary>
        /// <param name="dt">datatable的数据</param>
        /// <param name="targetPath">输出目录</param>
        public static void YXHDatatable2CSV_V1(this DataTable dt, string targetPath)
        {
            FileInfo? fi = new FileInfo(targetPath);
            if (fi.Directory != null && !fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            System.Text.StringBuilder sCsvContent;
            try
            {
                sCsvContent = new System.Text.StringBuilder();
                //栏位
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sCsvContent.Append(dt.Columns[i].ColumnName);
                    sCsvContent.Append(i == dt.Columns.Count - 1 ? "\r\n" : ",");
                }
                //数据
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
#pragma warning disable CS8602 // 解引用可能出现空引用。
                        sCsvContent.Append(row[i].ToString().Trim());
#pragma warning restore CS8602 // 解引用可能出现空引用。
                        sCsvContent.Append(i == dt.Columns.Count - 1 ? "\r\n" : ",");
                    }
                }
                File.WriteAllText(targetPath, sCsvContent.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 用于把内存中的datatable输出到指定目录下的csv文件的方式2，如目录不存在则会创建目录。
        /// </summary>
        /// <param name="dt">datatable的数据</param>
        /// <param name="targetPath">输出目录</param>
        public static void YXHDatatable2CSV_V2(this DataTable dt, string targetPath)
        {
            FileInfo fi = new FileInfo(targetPath);
            if (fi.Directory != null && !fi.Directory.Exists)
            {
                fi.Directory.Create();

            }
            FileStream fs = new FileStream(targetPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            //Tabel header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i].ColumnName);
                sw.Write("\t");
            }
            sw.WriteLine("");
            //Table body
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
#pragma warning disable CS8604 // 引用类型参数可能为 null。
                    sw.Write(DelQuota(dt.Rows[i][j].ToString()));
#pragma warning restore CS8604 // 引用类型参数可能为 null。
                    sw.Write("\t");
                }
                sw.WriteLine("");
            }
            sw.Flush();
            sw.Close();

        }
        /// <summary>
        /// （专用于结构化数据库拆分数据表和工况表）重命名dt列名
        /// </summary>
        /// <param name="dt">dt数据源</param>
        /// <param name="oldname">旧名</param>
        /// <param name="newname">新名</param>
        public static void YXHRenameDtColumn(this DataTable  dt, string  oldname, string newname)
        {
            if (dt!= null)
            {
                if (dt.Columns!= null)
                {
                    if (dt.Columns[oldname] != null)
                    {
                        dt.Columns[oldname].ColumnName = newname;
                    }
                }
            }
        }
        /// <summary>
        /// （专用于结构化数据库拆分数据表和工况表）修改筛选出来的数据值到dt
        /// </summary>
        /// <param name="dt">dt表</param>
        /// <param name="list">字典类数据集合</param>
        public static void YXHChangeDtValue(this DataTable dt, List<DictClass> list)
        {
            foreach (var l in list)
            {
                var s = dt.Select(string.Format("{0}='{1}'", l.name, l.key));
                if (s.Length > 0)
                {
                    foreach (var row in s)
                    {
                        row.BeginEdit();
                        if (l.name != null)
                        {
                            row[l.name] = l.value;
                        }
                        row.EndEdit(); //结束编辑
                    }
                }
                dt.AcceptChanges(); // 保存修改的结果
            }
        }
        /// <summary>
        /// （专用于结构化数据库拆分数据表和工况表）筛选出特定的列生成新的dt，用于ncode产生的csv数据里挑选出特定的列然后另存为新的csv
        /// </summary>
        /// <param name="dt">dt表</param>
        /// <param name="columns">特定列名数组</param>
        /// <returns>DataTable</returns>
        public static DataTable YXHSelectDataColumn2Dt(this DataTable dt, string[] columns)
        {
            return dt.DefaultView.ToTable(false, columns);
        }
        /// <summary>
        /// （专用于结构化数据库拆分数据表和工况表）专用于改写工况表dt的数据
        /// </summary>
        /// <param name="dt">dt表</param>
        /// <param name="projectid">项目名</param>
        /// <param name="vehicleid">车辆号</param>
        public static void YXHChangeConditionValue(this DataTable dt, string projectid, string vehicleid)
        {
            if (projectid != null && vehicleid != null)
            {
                var count = dt.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    dt.Rows[i]["ID"] = projectid + "-" + (i + 1).ToString("000");
                    dt.Rows[i]["ProjectID"] = projectid;
                    dt.Rows[i]["VehicleID"] = vehicleid;
                }
            }



        }
        public static string DelQuota(string str)
        {
            string result = str;
            string[] strQuota = { "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "`", ";", "'", ",", ".", "/", ":", "/,", "<", ">", "?" };
            for (int i = 0; i < strQuota.Length; i++)
            {
                if (result.IndexOf(strQuota[i]) > -1)
                    result = result.Replace(strQuota[i], "");
            }
            return result;
        }
        /// <summary>
        ///（专用于边缘采集器）读取一个csv文件到datatable，并把字段time的值根据index来修改成与上一个csv文件连续的值
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="index">文件序号（如第二个文件则time的第一个值就要考虑到index来连接上第一个文件time的最后一个数值）</param>
        /// <returns>DataTable</returns>
        public static DataTable YXHCSV2DataTable(this string fileName, int index)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            //记录每次读取的一行记录
            string? strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            int rowcount = 0;
            //逐行读取CSV中的数据
           
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    rowcount++;
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (j == 0)
                        {
                            dr[j] = internaltime * rowcount + index * 5;
                        }
                        else
                        {
                            dr[j] = aryLine[j];
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            sr.Close();
            fs.Close();
            return dt;
        }
        /// <summary>
        /// 此方法用于合并datatable数组成一个datatable
        /// </summary>
        /// <param name="dts">datatable数</param>
        /// <returns>DataTable</returns>
        public static DataTable YXHDataTableMerge(this DataTable[] dts)
        {
            DataTable dest = dts[0];
            for (int i = 1; i < dts.Length; i++)
            {
                for (int j = 0; j < dts[i].Rows.Count; j++)
                {
                    dest.ImportRow(dts[i].Rows[j]);
                }
            }
            return dest;
        }
        /// <summary>
        /// 把CSV转成datatable，不考虑time连续问题
        /// </summary>
        /// <param name="fileName">csv文件路径</param>
        /// <returns>DataTable</returns>
        public static DataTable YXHCSV2DataTable(this string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            //记录每次读取的一行记录
            string? strLine;
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {

                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {

                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        /// <summary>
        /// 打印datatable到console
        /// </summary>
        /// <param name="dt">datatable数据源</param>
        public static void YXHPrintDT2Console(this DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string columnStr = string.Empty;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    columnStr += dt.Rows[i][j] + " | ";
                }
                Console.WriteLine(columnStr);
            }
        }

        /// <summary>
        /// 把list里的数据追加到CSV文件中
        /// </summary>
        /// <param name="list">要转的list</param>
        /// <param name="outpath">输出的csv全路径</param>
        public static void YXHList2CSV(List<string[]> list, string outpath)
        {
            if (outpath != null)
            {
                FileInfo fi = new FileInfo(outpath);
                if (fi.Directory != null)
                {
                    if (!fi.Directory.Exists)
                    {
                        fi.Directory.Create();
                    }
                    for (var i = 0; i < list.Count; i++)
                    {
                        string content = "";
                        for (int j = 0; j < list[i].Length; j++)
                        {
                            if (j == list[i].Length - 1)
                            {
                                content += list[i][j] + "\r\n";
                            }
                            else
                            {
                                content += list[i][j] + ",";
                            }
                        }
                        File.AppendAllText(outpath, content);
                    }
                }
            }
           

        }
        /// <summary>
        /// 读取csv到List<string[]>，并需要修改time里的数据
        /// </summary>
        /// <param name="fileName">csv文件路径</param>
        /// <param name="index"></param>
        /// <returns>List<string[]></returns>
        public static List<string[]> YXHCSV2List(string fileName, int index)
        {
            List<string[]> list = new List<string[]>();
            //FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fileName, Encoding.UTF8);
            //记录每次读取的一行记录
            string? strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            //标示是否是读取的第一行
            bool IsFirst = true;
            int rowcount = 0;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                if (IsFirst == true)
                {
                    IsFirst = false;
                    if (index == 0)
                    {
                        list.Add(aryLine);
                    }
                }
                else
                {
                    rowcount++;
                    aryLine[0] = (internaltime * rowcount + index * 5).ToString();
                    list.Add(aryLine);
                }
            }
            sr.Close();
            sr.Dispose();
            return list;

        }
        /// <summary>
        /// 一次性的读取csv到List<string>
        /// </summary>
        /// <param name="fileInfo">FileInfo文件类</param>
        /// <returns>List<string></returns>
        public static async Task<List<string>> YXHReadCSV2ListOneTime(FileInfo fileInfo)
        {
            var r = await File.ReadAllLinesAsync(fileInfo.FullName);
            return r.ToList();
        }
        /// <summary>
        /// 一次性的保存成CSV
        /// </summary>
        /// <param name="filepath">保存路径</param>
        /// <param name="newlist">List数据</param>
        /// <returns></returns>
        public static async Task YXHSaveCSVfromListOneTime(string filepath, List<string> newlist)
        {
            await File.WriteAllLinesAsync(filepath, newlist);
        }
    }
}
