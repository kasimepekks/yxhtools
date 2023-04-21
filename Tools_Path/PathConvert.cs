using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YXH_Tools_Files.Tools_DirectoryandFile;

namespace YXH_Tools_Files.Tools_Path
{
    public static class PathConvert
    {
        public static string YXHpathConvert(this string path)
        {
            string outpath = "";
            if (path.Contains("\\"))
            {
                outpath = path.Replace("\\", "/");
            }
            return outpath;
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
    }

}
