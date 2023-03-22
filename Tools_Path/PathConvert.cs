using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
