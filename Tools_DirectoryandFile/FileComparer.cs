using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.Tools_DirectoryandFile
{
    public class FileComparer : IComparer
    {
        int IComparer.Compare(object? o1, object? o2)
        {
            FileInfo? fi1 = o1 as FileInfo;
            FileInfo? fi2 = o2 as FileInfo;
            if (fi1 != null && fi2 != null)
            {
                return fi1.Name.CompareTo(fi2.Name);
            }
            else
            {
                return 0;
            }
        }
    }
}
