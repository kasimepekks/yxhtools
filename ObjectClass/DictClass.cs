using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.ObjectClass
{
    /// <summary>
    /// 专用于结构化数据库字典里的数据与数字对应的关系类，如GD对应0
    /// </summary>
    public class DictClass
    {
        public string? name { get; set; }
        public string? key { get; set; }
        public string? value { get; set; }
    }
}
