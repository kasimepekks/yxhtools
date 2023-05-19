using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YXH_Tools_Files.Tools_CSV;
using YXH_Tools_Files.Tools_List;
using YXH_Tools_Files.Tools_Standard;

namespace YXH_Tools_Files.Tools_Excute
{
    public static class Tools_Excute_CSVSplitforDatabase
    {
        public static void YXHExcuteCSVSplit(string csvpath, string[] selectedcollums, string[] standard, string[] phase, string[] ballast, string conditioncsvmodelpath,string dataoutpath,string conditionoutpath)
        {
            Console.WriteLine("开始读取此路径：" + csvpath + " 下的CSV文件");
            var dt = CSVOperator.YXHCSV2DataTable(csvpath);
            var projectid = dt.Rows[1][0].ToString();
            var vehicleid = dt.Rows[1][7].ToString();
            var standardtype=dt.Rows[1][6].ToString();//获取database里的standard的值，用于后面工况循环数的创建
            int stype = 0;
            if (standardtype != null)
            {
                stype = standardtype.standardclassify();//获取standard的int值
            }
            
            dt.YXHRenameDtColumn("ChanIDN", "DataID");
            dt.YXHRenameDtColumn("YUnits", "Units");
            var s = ListOperation.YXHCreateDictClass("Standard", standard);
            var p = ListOperation.YXHCreateDictClass("Phase", phase);
            var b = ListOperation.YXHCreateDictClass("Ballast", ballast);
            dt.YXHChangeDtValue(s);
            dt.YXHChangeDtValue(p);
            dt.YXHChangeDtValue(b);
            var st = dt.YXHSelectDataColumn2Dt(selectedcollums);
            CSVOperator.YXHDatatable2CSV_V1(st, dataoutpath);
            var dtc = CSVOperator.YXHCSV2DataTablebyStandardtype(conditioncsvmodelpath,stype);//工况表模版csv
            if(projectid!=null&&vehicleid!=null)
            dtc.YXHChangeConditionValue(projectid, vehicleid);
            CSVOperator.YXHDatatable2CSV_V1(dtc, conditionoutpath);
            Console.WriteLine("已生成在此路径：" + dataoutpath + " 下的CSV文件");
            Console.WriteLine("你可以关闭控制台了，还愣着干嘛！！！");
            Console.ReadLine();
        }
    }
}
