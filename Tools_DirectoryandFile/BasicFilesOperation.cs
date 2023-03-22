namespace YXH_Tools_Files.Tools_DirectoryandFile
{
    public static class BasicFilesOperation
    {
        /// <summary>
        /// 返回目标文件夹下的所有子文件夹，不递归
        /// </summary>
        /// <param name="directorypath">文件夹路径</param>
        /// <returns>DirectoryInfo[]</returns>
        public static DirectoryInfo[] YXHGetSubDirectories(string directorypath)
        {
            DirectoryInfo root = new DirectoryInfo(directorypath);
            return root.GetDirectories();
        }
        /// <summary>
        /// 返回目标文件夹下的所有指定的ext格式文件，并按文件名排序（如无此文件夹会自动创建）
        /// </summary>
        /// <param name="directorypath">文件夹路径</param>
        /// <param name="ext">指定文件扩展名</param>
        /// <returns>FileInfo[]</returns>
        public static FileInfo[] YXHGetAllextFiles(string directorypath, string ext)
        {
            DirectoryInfo root = new DirectoryInfo(directorypath);
            if (!root.Exists)
            {
                root.Create();
            }
            //getfiles始终按照文件名排序
            FileInfo[] files = root.GetFiles(ext);
            //if (files.Length > 0)
            //{
            //    var fc = new FileComparer();
            //    Array.Sort(files, fc);
            //}
            return files;
        }
        /// <summary>
        /// 判断文件是否被使用
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns>bool</returns>
        public static bool YXHCheckFileIsUsed(string filepath)
        {
            bool result = false;
            //判断文件是否存在，如果不存在，直接返回 false
            if (!File.Exists(filepath))
            {
                result = false;
            }//end: 如果文件不存在的处理逻辑
            else
            {//如果文件存在，则继续判断文件是否已被其它程序使用
                //逻辑：尝试执行打开文件的操作，如果文件已经被其它程序使用，则打开失败，抛出异常，根据此类异常可以判断文件是否已被其它程序使用。
                FileStream? fileStream = null;
                try
                {
                    fileStream = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    result = false;
                }
                catch (IOException)
                {
                    result = true;
                }
                catch (Exception)
                {
                    result = true;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filepath">文件路径</param>  
        public static void YXHDeleteFile(string filepath)
        {
            File.Delete(filepath);
        }
        /// <summary>
        /// 用于把斜杠转成path下的斜杠，这样就直接可以在windows地址栏中复制路径了
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string YXHPathReplace(this string path)
        {
            if (path.Contains("\\"))
            {
                return path.Replace("\\", "/");
            }
            else
            {
                return path;
            }
        }
    }
}