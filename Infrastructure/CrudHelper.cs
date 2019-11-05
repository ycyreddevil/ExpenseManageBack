using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yuyu.Infrastructure;
using System.Text;
using System.Data;
using System.Reflection;

namespace ExpenseManageBack.Infrastructure
{
    public class CrudHelper<T> where T : new()
    {
        SqlResult Result { get; set; }
        string TableName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="talbeName">类名</param>
        public CrudHelper(string talbeName)
        {
            TableName = talbeName;
        }

        public SqlResult Add(T modul,bool NeedAddId=false)
        {
            List<T> list = new List<T>();
            list.Add(modul);
            Add(list,NeedAddId);
            return Result;
        }

        public SqlResult Add(List<T> listModul, bool NeedAddId=false)
        {
            if (listModul == null || listModul.Count == 0)
            {
                return Result;
            }
            Type t = listModul[0].GetType();
            DataTable dt = FillDataTable(listModul);
            if (dt.Columns.Contains("Id") && !NeedAddId)
                dt.Columns.Remove("Id");
            string sql = SqlHelper.GetInsertString(dt, TableName);
            Result = new SqlResult(SqlHelper.Exce(sql));
            return Result;
        }


        public SqlResult Update(T modul)
        {
            List<T> list = new List<T>();
            list.Add(modul);
            Update(list);
            return Result;
        }

        public SqlResult Update(List<T> listModul)
        {
            if (listModul == null || listModul.Count == 0)
            {
                return Result;
            }
            Type t = listModul[0].GetType();
            DataTable dt = FillDataTable(listModul);
            List<string> listSql = new List<string>();
            foreach(DataRow row in dt.Rows)
            {
                string sql = SqlHelper.GetUpdateString(row, TableName, string.Format(" where Id={0}",row["Id"].ToString()));
                listSql.Add(sql);
            }
            
            Result = new SqlResult(SqlHelper.Exce(listSql.ToArray()));
            return Result;
        }

        public SqlResult Delete(int Id)
        {
            string sql = string.Format("delete from {0} where Id={1}",TableName, Id);
            Result = new SqlResult(SqlHelper.Exce(sql));
            return Result;
        }

        public SqlResult Delete(string condition="1=1")
        {
            string sql = string.Format("delete from {0} where {1}", TableName, condition);
            Result = new SqlResult(SqlHelper.Exce(sql));
            return Result;
        }

        public SqlResult Delete(int[] Id)
        {
            if(Id.Length==0)
            {
                return Result;
            }
            string Ids = "";
            foreach(int id in Id)
            {
                Ids += id.ToString() + ",";
            }
            Ids = Ids.Substring(0, Ids.Length - 1);
            string sql = string.Format("delete from {0} where Id in ({1})", TableName, Ids);
            Result = new SqlResult(SqlHelper.Exce(sql));
            return Result;
        }

        public T GetModel(int Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM " + TableName);
            strSql.Append(" where Id=" + Id.ToString());
            DataSet ds = SqlHelper.Find(strSql.ToString());
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new T();
            else
                return FillModel(ds.Tables[0].Rows[0]);

        }

        public List<T> GetList(string condition = "1=1")
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM " + TableName);
            strSql.Append(" where " + condition);
            DataSet ds = SqlHelper.Find(strSql.ToString());
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return FillModel(ds.Tables[0]);
        }

        public List<T> GetListBySql(string sql )
        {
            DataSet ds = SqlHelper.Find(sql);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return FillModel(ds.Tables[0]);
        }

        /// <summary>
        /// 实体类转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable FillDataTable(List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = CreateDataStruct(modelList[0]);

            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        private static DataTable CreateDataStruct(T model)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
            }
            return dataTable;
        }

        /// <summary>  
        /// 填充对象：用DataRow填充实体类
        /// </summary>  
        public static T FillModel(DataRow dr)
        {
            if (dr == null)
            {
                return default(T);
            }

            //T model = (T)Activator.CreateInstance(typeof(T));  
            T model = new T();

            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                //PropertyInfo[] ps = model.GetType().GetProperties();
                PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                if (propertyInfo != null && dr[i] != DBNull.Value)
                    propertyInfo.SetValue(model, dr[i], null);
            }
            return model;

            //T t = new T();
            //string tempName = "";
            //// 获得此模型的公共属性  
            //PropertyInfo[] propertys = t.GetType().GetProperties();

            //foreach (PropertyInfo pi in propertys)
            //{
            //    tempName = pi.Name;

            //    // 检查DataTable是否包含此列  
            //    if (dr.Columns.Contains(tempName))
            //    {
            //        // 判断此属性是否有Setter  
            //        if (!pi.CanWrite) continue;

            //        object value = dr[tempName];
            //        if (value != DBNull.Value)
            //            pi.SetValue(t, value, null);
            //    }
            //}
        }


        /// <summary>  
        /// 填充对象列表：用DataTable填充实体类
        /// </summary>  
        public static List<T> FillModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                //T model = (T)Activator.CreateInstance(typeof(T));  
                T model = new T();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }

                modelList.Add(model);
            }
            return modelList;
        }

    }
}
