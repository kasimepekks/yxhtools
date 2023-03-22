using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.Tools_EdgeComputing
{
    /// <summary>
    /// FileInfo文件名转时间操作类
    /// </summary>
    public static class FileInfoOperation_TimeTransfer
    {
        /// <summary>
        ///（专用于边缘采集器csv文件） 根据文件名来返回完整的文件日期，例如：2022-02-12 08:12:23
        /// </summary>
        /// <param name="file">FileInfo类型文件</param>
        /// <returns>DateTime</returns>
        public static DateTime YXHFilenametoDateTime(this FileInfo file)
        {
            string date = file.Name.Split("-")[0].Replace("_", "-");
            string time = file.YXHFilenametoHour() + ":" + file.YXHFilenametoMinute() + ":" + file.YXHFilenametoSecond();
            return Convert.ToDateTime(date + " " + time);
        }
        /// <summary>
        /// （专用于边缘采集器csv文件）根据文件名来返回文件日期，例如：2022-02-12
        /// </summary>
        /// <param name="file">FileInfo类型文件</param>
        /// <returns>string</returns>
        public static string YXHFilenametoDate(this FileInfo file)
        {
            return file.Name.Split("-")[0].Replace("_", "-");
        }
        /// <summary>
        /// （专用于边缘采集器csv文件）根据文件名来返回时间几点，例如：08点
        /// </summary>
        /// <param name="file">FileInfo类型文件</param>
        /// <returns>string</returns>
        public static string YXHFilenametoHour(this FileInfo file)
        {
            return file.Name.Split("-")[1].Split("_")[0];
        }
        /// <summary>
        /// （专用于边缘采集器csv文件）根据文件名来返回时间几分，例如：10分
        /// </summary>
        /// <param name="file">FileInfo类型文件</param>
        /// <returns>string</returns>
        public static string YXHFilenametoMinute(this FileInfo file)
        {
            return file.Name.Split("-")[1].Split("_")[1];
        }
        /// <summary>
        ///（专用于边缘采集器csv文件） 根据文件名来返回时间几秒，例如：55秒
        /// </summary>
        /// <param name="file">FileInfo类型文件</param>
        /// <returns></returns>
        public static string YXHFilenametoSecond(this FileInfo file)
        {
            return file.Name.Split("-")[1].Split("_")[2];
        }
    }
}
