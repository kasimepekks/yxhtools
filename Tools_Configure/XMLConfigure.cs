using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.Tools_Configure
{
    public static class XMLConfigure
    {
        /// <summary>
        /// 获取配置文件指定的Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string? YXHGetXMLConfigKey(this string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        /// <summary>
        /// 获取配置文件指定的section和key的value
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns>string</returns>
        public static string? YXHGetXMLConfigSection(this string section,string key)
        {
            IDictionary sectiondict = (IDictionary)ConfigurationManager.GetSection(section);
            string[] keys = new string[sectiondict.Keys.Count];
            string[] values = new string[sectiondict.Values.Count];
            sectiondict.Keys.CopyTo(keys, 0);
            sectiondict.Values.CopyTo(values, 0);
            if (keys.Contains(key)){
                var keyindex = keys.ToList().IndexOf(key);
                return values[keyindex];
            }
            else
            {
                return null;
            }
        }
    }
}
