using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace yuyu.Infrastructure
{
    public class DataTableHelper
    {
        /// <summary>  
        /// dataTable转换成Json格式  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTable2Json(DataTable dt)
        {
            var arrayList = new ArrayList();
            foreach (DataRow dataRow in dt.Rows)
            {
                var dictionary = new Dictionary<string, object>();  //实例化一个参数集合
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
                }
                arrayList.Add(dictionary); //ArrayList集合中添加键值
            }

            return JsonConvert.SerializeObject(arrayList);
        }

        /// <summary>
        /// dataTable转换成JArray格式  
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static JArray DataTable2JArray(DataTable dt)
        {
            var jArray = new JArray();
            foreach (DataRow dataRow in dt.Rows)
            {
                var jObject = new JObject();
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    jObject.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
                }
                jArray.Add(jObject);
            }

            return jArray;
        }
    }
}