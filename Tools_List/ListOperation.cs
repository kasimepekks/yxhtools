using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YXH_Tools_Files.ObjectClass;

namespace YXH_Tools_Files.Tools_List
{
    /// <summary>
    /// （ncode的csv表头操作）专用于解决删除ncode转成的csv表头问题，使csv与边缘采集器的csv表头一致
    /// </summary>
    public static class ListOperation
    {
        /// <summary>
        /// （ncode的csv表头操作）删除多余的标题行，添加time标题再返回
        /// </summary>
        /// <param name="list">要编辑的List</param>
        /// <returns>List<string></returns>
        public static List<string> YXHEditCsvTitle(ref List<string> list)
        {
            list.RemoveRange(3, 5);//删除第四到6行
            list.RemoveRange(0, 2);//删除第一第二行
            var s = list[0].Split(",").ToList();//把title行用逗号分开
            s.RemoveRange(0, 2);//删除title行前面两列空的
            s.Insert(0, "Time");//添加第一列time
            var newtitle = string.Join(",", s.ToArray());//改为新的csv字段名
            list[0] = newtitle;//重新赋给list[0]
            return list;
        }
        /// <summary>
        /// （ncode的csv表头操作）专用于从第二行开始的每行数据删除第一列的序号数据再返回
        /// </summary>
        /// <param name="list">要编辑的List</param>
        /// <returns>List<string></returns>
        public static List<string> YXHEditCsvDataSP(ref List<string> list)
        {
            //把从第二行开始的每行删除第一列的数据
            for (int i = 1; i < list.Count; i++)
            {
                var l = list[i].Split(",").ToList();
                l.RemoveAt(0);
                list[i] = string.Join(",", l.ToArray());
            }
            return list;
        }
        /// <summary>
        /// （专用于获得cuma数据去掉表头后的文件名）获得文件名如"2019_12_09-10_01_00-1-F.csv"
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="settingname"></param>
        /// <returns>string</returns>
        public static string YXHEditCsvName(FileInfo fileInfo, string settingname)
        {
            var oldname = fileInfo.Name.Split(".")[0];//先获得文件名如“20191209_1_6_input”
            var datestring = oldname.Split("_")[0];//再获得文件名如"20191209"
            var t1 = datestring.Insert(6, "_");//再获得文件名如"201912_09"
            var t2 = t1.Insert(4, "_");//再获得文件名如"2019_12_09"
            return t2.ToString() + "-" + settingname + "-1-F.csv";//再获得文件名如"2019_12_09-10_01_00-1-F.csv"
        }
        /// <summary>
        /// （专用于结构化数据库拆分数据表和工况表）创建List<DictClass>,用于后面的修改dt里的值
        /// </summary>
        /// <param name="name">字典类名，如Standard</param>
        /// <param name="s">类名里的每一项名，如GD</param>
        /// <returns>List<DictClass></returns>
        public static List<DictClass> YXHCreateDictClass(string name, string[] s)
        {
            List<DictClass> list = new List<DictClass>();
            for (int i = 0; i < s.Length; i++)
            {
                DictClass dc = new DictClass();
                dc.name = name;
                dc.key = s[i];
                dc.value = i.ToString();
                list.Add(dc);
            }
            return list;
        }

    }
}
