using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace ExpenseManageBack.Infrastructure
{
    public class SqlHelper
    {
        //private static string connectString = System.Configuration.ConfigurationManager.AppSettings["connectionstring"];

        //private static string database = System.Configuration.ConfigurationManager.AppSettings["databaseName"];
        private static string connectString = AppSettingHelper.GetSection("ConnectionStrings:yuyuDBContext");

        public static string ConnectString
        {
            get
            {
                return connectString;
            }

            set
            {
                connectString = value;
            }
        }

        public static DataSet Find(string sql)
        {
            string str = "";
            return Find(sql, ref str);
        }

        public static DataSet Find(string[] sql)
        {
            string str = "";
            return Find(sql, ref str);
        }

        public static DataSet Find(string[] sql, ref string msg)
        {
            DataSet ds = new DataSet();
            msg = "";
            using (MySqlConnection connection = new MySqlConnection(ConnectString))
            {
                try
                {
                    connection.Open();
                    foreach (string strCmd in sql)
                    {
                        if (string.IsNullOrEmpty(strCmd))
                        {
                            continue;
                        }
                        DataTable dt = new DataTable();

                        MySqlDataAdapter adapter = new MySqlDataAdapter(strCmd, connection);
                        adapter.Fill(dt);


                        ds.Tables.Add(dt);
                    }
                }
                catch (Exception ex)
                {
                    ds = null;
                    //throw new Exception(ex.Message);
                    msg = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            return ds;
        }


        public static DataSet Find(string sql, ref string msg)
        {
            DataSet ds = new DataSet();
            msg = "";
            using (MySqlConnection connection = new MySqlConnection(ConnectString))
            {
                string[] strs = sql.Split(';');
                try
                {
                    connection.Open();
                    foreach (string strCmd in strs)
                    {
                        if (string.IsNullOrEmpty(strCmd))
                        {
                            continue;
                        }
                        DataTable dt = new DataTable();

                        MySqlDataAdapter adapter = new MySqlDataAdapter(strCmd, connection);
                        adapter.Fill(dt);


                        ds.Tables.Add(dt);
                    }
                }
                catch (Exception ex)
                {
                    ds = null;
                    //throw new Exception(ex.Message);
                    msg = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            return ds;
        }

        public static SqlExceResult[] Exce(string sql)
        {
            List<SqlExceResult> res = new List<SqlExceResult>();
            using (MySqlConnection connection = new MySqlConnection(ConnectString))
            {
                int cnt = 0;

                string[] strs = sql.Split(';');
                connection.Open();
                foreach (string strCmd in strs)
                {
                    if (string.IsNullOrEmpty(strCmd))
                    {
                        continue;
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strCmd, connection))
                    {
                        try
                        {
                            cnt = cmd.ExecuteNonQuery();
                            //ret += string.Format("{0}:{1},操作成功;\r\n", cmdCount, cnt);
                            res.Add(new SqlExceResult(0, cnt, "success", strCmd));
                        }
                        catch (MySql.Data.MySqlClient.MySqlException e)
                        {
                            //ret += string.Format("{0}:{1},{2};\r\n", cmdCount, cnt, e.ToString());
                            res.Add(new SqlExceResult(1, 0, e.ToString(), strCmd));
                        }
                    }
                }
                connection.Close();
            }
            return res.ToArray();
        }

        public static SqlExceResult[] Exce(string[] sql)
        {
            List<SqlExceResult> res = new List<SqlExceResult>();
            using (MySqlConnection connection = new MySqlConnection(ConnectString))
            {
                int cnt = 0;

                connection.Open();
                foreach (string strCmd in sql)
                {
                    if (string.IsNullOrEmpty(strCmd))
                    {
                        continue;
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strCmd, connection))
                    {
                        try
                        {
                            cnt = cmd.ExecuteNonQuery();
                            //ret += string.Format("{0}:{1},操作成功;\r\n", cmdCount, cnt);
                            if(cnt >0)
                                res.Add(new SqlExceResult(0, cnt, "success", strCmd));
                            else
                                res.Add(new SqlExceResult(1, 0, "repetitive", strCmd));
                        }
                        catch (MySql.Data.MySqlClient.MySqlException e)
                        {
                            //ret += string.Format("{0}:{1},{2};\r\n", cmdCount, cnt, e.ToString());
                            res.Add(new SqlExceResult(2, 0, e.ToString(), strCmd));
                        }
                    }
                }
                connection.Close();
            }
            return res.ToArray();
        }

        public static string InsertAndGetLastId(string sql)
        {
            JObject res = new JObject();
            using (MySqlConnection connection = new MySqlConnection(ConnectString))
            {
                connection.Open();
                int cnt = 0;
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    try
                    {
                        cnt = cmd.ExecuteNonQuery();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        //sql执行报错
                        res.Add("Success", "-1");
                        res.Add("message", e.ToString());
                        return res.ToString();
                    }
                }
                sql = "SELECT LAST_INSERT_ID()";
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    if (cnt > 0)//已插入成功
                    {
                        object obj = null;
                        try
                        {
                            obj = cmd.ExecuteScalar();
                            res.Add("Success", "1");
                            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                                res.Add("Id", "-1");
                            else
                                res.Add("Id", obj.ToString());
                        }
                        catch (Exception e)
                        {
                            res.Add("Success", "-2");
                            res.Add("message", e.ToString());
                            return res.ToString();
                        }
                    }
                    else//sql 无错误，但有重复，插入不成功
                    {
                        res.Add("Success", "0");
                        res.Add("message", "插入数据失败，有重复索引!");
                    }
                }
                connection.Close();
            }
            return res.ToString();
        }

        public static object Scalar(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static string[] Scalar(string[] sqls)
        {
            List<string> list = new List<string>();
            using (MySqlConnection connection = new MySqlConnection(ConnectString))
            {
                connection.Open();
                foreach (string sql in sqls)
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        try
                        {
                            object obj = cmd.ExecuteScalar();
                            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            {
                                list.Add("");
                            }
                            else
                            {
                                list.Add(obj.ToString());
                            }
                        }
                        catch (MySql.Data.MySqlClient.MySqlException e)
                        {
                            //throw e;
                            list.Clear();
                            break;
                        }
                    }
                }
                connection.Close();
            }
            return list.ToArray();
        }

        public static string GetSaveString(Dictionary<string, string> dict, string tableName, string condition)
        {
            string sql = string.Format("select * from {0} {1}", tableName, condition);
            DataSet ds = Find(sql);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                sql = GetInsertIgnoreString(dict, tableName);
            else
                sql = GetUpdateString(dict, tableName, condition);
            return sql;
        }

        public static string GetInsertIgnoreString(DataRow row, string table)
        {
            string res = GetInsertString(row, table);
            res = res.Replace("Insert", "Insert ignore ");//避免重复插入数据
            return res;
        }

        public static string GetInsertIgnoreString(Dictionary<string, string> dict, string table)
        {
            string res = GetInsertString(dict, table);
            res = res.Replace("Insert", "Insert ignore ");//避免重复插入数据
            return res;
        }

        public static string GetInsertIgnoreString(DataTable dt, string table)
        {
            string res = GetInsertString(dt, table);
            res = res.Replace("Insert", "Insert ignore ");//避免重复插入数据
            return res;
        }

        public static string GetSaveString(DataTable dt, string table, string IdColumn)
        {
            if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0)
            {
                return "";
            }
            string sql = "";

            foreach (DataRow row in dt.Rows)
            {
                string insert = GetInsertString(row, table);
                string update = GetUpdateString(row, table, string.Format(" where {0}='{1}'", IdColumn, row[IdColumn].ToString()));
                sql += string.Format("if not exists (select {0} from {1} where {2}='{3}')"
                    , IdColumn, table, IdColumn, row[IdColumn].ToString());
                sql += " begin " + insert + " end ";
                sql += " else begin " + update + " end\r\n;";
            }
            return sql;
        }

        public static string GetInsertString(DataRow row, string table)
        {
            if (row == null || row.ItemArray.Length == 0)
            {
                return "";
            }
            string fileds = "";
            string values = "";
            foreach (DataColumn clm in row.Table.Columns)
            {
                fileds += string.Format("{0},", VerifyString(clm.ColumnName));
                values += string.Format("'{0}',", VerifyString(row[clm.ColumnName].ToString()));
            }
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            string sql = string.Format("Insert into {0} ({1}) values ({2})", table, fileds, values);
            return sql;
        }

        public static string GetInsertString(ArrayList list, string table)
        {
            if (list == null || list.Count == 0)
            {
                return "";
            }

            string sql = "";
            int len = list.Count;

            string fileds = "";
            string values = "";
            Dictionary<string, string> dict = (Dictionary<string, string>)list[0];
            if (dict.Keys.Count == 0)
                return "";
            foreach (string key in dict.Keys)
            {
                fileds += string.Format("{0},", VerifyString(key));
                values += string.Format("'{0}',", VerifyString(dict[key]));
            }
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            sql += string.Format("Insert into {0} ({1}) values ({2})", table, fileds, values);

            for (int i = 1; i < len; i++)
            {
                dict = (Dictionary<string, string>)list[i];
                values = "";
                foreach (string key in dict.Keys)
                {
                    values += string.Format("'{0}',", VerifyString(dict[key]));
                }
                values = values.Substring(0, values.Length - 1);
                sql += string.Format(", ({0})", values);
            }
            sql += "\r\n;";
            return sql;
        }

        public static string GetInsertString(List<JObject> list, string table)
        {
            if (list == null || list.Count == 0)
            {
                return "";
            }

            string sql = "";
            int len = list.Count;

            string fileds = "";
            string values = "";
            JObject dict = (JObject)list[0];
            foreach (var jp in dict)
            {
                fileds += string.Format("{0},", VerifyString(jp.Key));
                values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
            }
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            sql += string.Format("Insert into {0} ({1}) values ({2})", table, fileds, values);

            for (int i = 1; i < len; i++)
            {
                dict = (JObject)list[i];
                values = "";
                foreach (var jp in dict)
                {
                    values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
                }
                values = values.Substring(0, values.Length - 1);
                sql += string.Format(", ({0})", values);
            }
            sql += "\r\n;";
            return sql;
        }

        public static string GetInsertString(Dictionary<string, string> dict, string table)
        {
            if (dict.Count == 0)
            {
                return "";
            }
            string fileds = "";
            string values = "";
            foreach (string key in dict.Keys)
            {
                fileds += string.Format("{0},", VerifyString(key));
                //if(key == "Id")
                //    values += string.Format("{0},", VerifyString(dict[key]));
                //else
                values += string.Format("'{0}',", VerifyString(dict[key]));
            }
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            string sql = string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
            return sql;
        }

        public static string GetInsertString(JObject dict, string table)
        {
            if (dict.Count == 0)
            {
                return "";
            }
            string fileds = "";
            string values = "";
            foreach (var jp in dict)
            {
                fileds += string.Format("{0},", VerifyString(jp.Key));
                values += string.Format("'{0}',", VerifyString(jp.Value.ToString()));
            }
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            string sql = string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
            return sql;
        }

        public static string GetInsertString(DataTable dt, string table)
        {
            if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0)
            {
                return "";
            }
            string sql = "";
            int len = dt.Rows.Count;

            string fileds = "";
            string values = "";
            DataRow row = dt.Rows[0];
            foreach (DataColumn clm in dt.Columns)
            {
                fileds += string.Format("{0},", VerifyString(clm.ColumnName));
                values += string.Format("'{0}',", VerifyString(row[clm.ColumnName].ToString()));
            }
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            sql += string.Format("Insert into {0} ({1}) values ({2})", table, fileds, values);

            for (int i = 1; i < len; i++)
            {
                row = dt.Rows[i];
                values = "";
                foreach (DataColumn clm in dt.Columns)
                {
                    values += string.Format("'{0}',", VerifyString(row[clm.ColumnName].ToString()));
                }
                values = values.Substring(0, values.Length - 1);
                sql += string.Format(", ({0})", values);
            }
            sql += "\r\n;";
            //foreach (DataRow row in dt.Rows)
            //{
            //    string fileds = "";
            //    string values = "";
            //    foreach (DataColumn clm in dt.Columns)
            //    {
            //        fileds += string.Format("{0},", VerifyString(clm.ColumnName));
            //        values += string.Format("'{0}',", VerifyString(row[clm.ColumnName].ToString()));
            //    }
            //    if(dt.Columns.Count>0)
            //    {
            //        fileds = fileds.Substring(0, fileds.Length - 1);
            //        values = values.Substring(0, values.Length - 1);
            //        sql += string.Format("Insert into {0} ({1}) values ({2}) \r\n;", table, fileds, values);
            //    }            
            //}       

            return sql;
        }

        public static string GetMassUpdateString(ArrayList list, string table, string condition)
        {
            //JObject jobj = new JObject();
            //jobj.Add("Index", 0);
            //jobj.Add("ErrMsg", "无数据");
            if (list.Count == 0)
            {
                return "";
            }
            string values = "";
            string fileds = "";
            foreach (string key in ((Dictionary<string, string>)list[0]).Keys)
            {
                fileds += key + ", ";
            }
            fileds = fileds.Substring(0, fileds.Length - 2);
            string sql = string.Format("replace into {0} ({1}) values ", table, fileds);
            foreach (Dictionary<string, string> dict in list)
            {
                values = "(";
                foreach (string key in dict.Keys)
                {
                    values += dict[key] + ", ";
                }
                values = values.Substring(0, fileds.Length - 2);
                values += "),";
                sql += values;
            }
            sql = sql.Substring(0, fileds.Length - 1);
            return sql;
        }

        public static string GetUpdateString(ArrayList list, string tableName, string condition)
        {
            if (list.Count == 0)
            {
                return "";
            }
            string sql = "";
            foreach (Dictionary<string, string> dict in list)
            {
                sql += string.Format("Update {0} set ", tableName);
                foreach (string key in dict.Keys)
                {
                    sql += string.Format("{0}='{1}', ", VerifyString(key)
                        , VerifyString(dict[key]));
                }
                sql = sql.Substring(0, sql.Length - 2);
                sql += string.Format(" {0}\r\n;", condition);
            }
            return sql;
        }

        public static string GetUpdateString(ArrayList list, string tableName, List<string> condition)
        {
            if (list.Count == 0)
            {
                return "";
            }
            string sql = "";
            int index = 0;
            foreach (Dictionary<string, string> dict in list)
            {
                sql += string.Format("Update {0} set ", tableName);
                foreach (string key in dict.Keys)
                {
                    sql += string.Format("{0}='{1}', ", VerifyString(key)
                        , VerifyString(dict[key]));
                }
                sql = sql.Substring(0, sql.Length - 2);
                sql += string.Format(" {0}\r\n;", condition[index]);
                index++;
            }
            return sql;
        }
        public static string GetUpdateString(DataTable dt, string tableName, string condition)
        {
            if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0)
            {
                return "";
            }
            string sql = "";
            foreach (DataRow row in dt.Rows)
            {
                sql += string.Format("Update {0} set ", tableName);
                foreach (DataColumn clm in dt.Columns)
                {
                    sql += string.Format("{0}='{1}', ", VerifyString(clm.ColumnName)
                        , VerifyString(row[clm.ColumnName].ToString()));
                }
                sql = sql.Substring(0, sql.Length - 2);
                sql += string.Format(" {0}\r\n;", condition);
            }
            return sql;
        }

        public static string GetUpdateString(DataRow row, string tableName, string condition)
        {
            if (row == null || row.ItemArray.Length == 0)
            {
                return "";
            }
            string sql = string.Format("Update {0} set ", tableName);
            foreach (DataColumn clm in row.Table.Columns)
            {
                sql += string.Format("{0}='{1}', ", VerifyString(clm.ColumnName)
                        , VerifyString(row[clm.ColumnName].ToString()));
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += string.Format(" {0}", condition);
            return sql;
        }

        public static string GetUpdateString(Dictionary<string, string> dict, string tableName, string condition)
        {
            if (dict.Count == 0)
            {
                return "";
            }
            string sql = string.Format("Update {0} set ", tableName);
            foreach (string key in dict.Keys)
            {
                sql += string.Format("{0}='{1}', ", VerifyString(key), VerifyString(dict[key]));
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += string.Format(" {0}\r\n;", condition);
            return sql;
        }

        public static string GetUpdateString(JObject dict, string tableName, string condition)
        {
            if (dict.Count == 0)
            {
                return "";
            }
            string sql = string.Format("Update {0} set ", tableName);
            foreach (var jp in dict)
            {
                sql += string.Format("{0}='{1}', ", VerifyString(jp.Key.ToString()), VerifyString(jp.Value.ToString()));
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql += string.Format(" {0}\r\n;", condition);
            return sql;
        }

        private static string VerifyString(string str)
        {
            if (!string.IsNullOrEmpty(str))
                str = str.Replace(";", "；");
            return str;
        }
    }

    public class SqlExceResult
    {
        public int Code { get; set; }
        public int LineCount { get; set; }
        public string Message { get; set; }
        public string SqlCmd { get; set; }

        public SqlExceResult(int code,int count,string msg,string sql)
        {
            Code = code;
            LineCount = count;
            Message = msg;
            SqlCmd = sql;
        }
    }

    public class SqlResult
    {
        public bool IsAllSuccess { get; set; }
        public List<SqlExceResult> ErrList { get; set; }

        public SqlResult(SqlExceResult[] list)
        {
            IsAllSuccess = true;
            foreach(SqlExceResult res in list)
            {
                if(res.Code != 0)
                {
                    IsAllSuccess = false;
                    ErrList.Add(res);
                }
            }
        }
    }
}
