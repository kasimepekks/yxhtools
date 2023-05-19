using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.Tools_Standard
{
    public static class StandardClassify
    {
        public static int standardclassify(this string standard)
        {
            int result;
            switch (standard)
            {
                case "GD":
                    result= 0;
                    break;
                case "SD":
                    result = 1;
                    break;
            
                case "ATPV":
                    result = 2;
                    break;
                case "PTD":
                    result = 4;
                    break;
                case "EGD":
                    result = 5;
                    break;
                case "ESGD":
                    result = 6;
                    break;
                case "SSD":
                    result = 7;
                    break;
                default:
                    result = 0;
                    break;
            }
            return result;
        }
    }
}
