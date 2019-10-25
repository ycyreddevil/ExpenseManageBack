using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ExpenseManageBack.Infrastructure
{
    public class CookieHelper
    {
        private HttpContext Context = null;
        public CookieHelper(HttpContext context)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            Context = context;
        }

      



        /// <summary>
        /// 添加cookie缓存不设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddCookie(string key, string value)
        {
            try
            {
                Context.Response.Cookies.Append(key, value);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 添加cookie缓存设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time">从当前时间开始后毫秒数</param>
        public void AddCookie(string key, string value, int time)
        {
            Context.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMilliseconds(time)
            });
        }
        /// <summary>
        /// 添加cookie缓存设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time">从当前时间开始后毫秒数</param>
        public void AddCookie(string key, string value, DateTime time)
        {
            Context.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = time
            });
        }
        /// <summary>
        /// 删除cookie缓存
        /// </summary>
        /// <param name="key"></param>
        public void DeleteCookie(string key)
        {
            Context.Response.Cookies.Delete(key);
        }
        /// <summary>
        /// 根据键获取对应的cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            var value = "";
            Context.Request.Cookies.TryGetValue(key, out value);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }
            return value;
        }
    }
}
