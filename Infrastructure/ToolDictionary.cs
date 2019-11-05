using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseManageBack.Infrastructure
{
    public class ToolDictionary
    {
        public static object GetValue(string key,Dictionary<string,object> dict)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            else
                return null;
        }

        public static string GetValueString(string key, Dictionary<string, object> dict)
        {
            if (dict.ContainsKey(key))
            {
                if (dict[key] == null)
                    return "";
                else
                    return dict[key].ToString();
            }
                
            else
                return "";
        }
    }
}
