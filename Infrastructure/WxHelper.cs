using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using yuyu.Infrastructure;
using ExpenseManageBack.Service;
using Newtonsoft.Json.Linq;

namespace ExpenseManageBack.Infrastructure
{
    public class WxHelper
    {
        private string CorpId = "";
        private string AppSecret = "";
        private string AgentId = "";
        private string AppName = "";
        private HttpContext Context;
        private int UserInfoSaveCookieDays = 30;
        public string RedirectUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">企业微信应用名称</param>
        /// <param name="redirectUri"></param>
        public WxHelper(HttpContext context, string name = "app1", string redirectUri = "")
        {
            if(!string.IsNullOrEmpty(name))
            {
                WxParameter wxP = new WxParameter(name);
                AppSecret = wxP.App.Secret;
                AgentId = wxP.App.AgentId;
                CorpId = wxP.CorpId;
                AppName = name;
                UserInfoSaveCookieDays = wxP.UserInfoSaveCookieDays;
            }
            
            Context = context;
            if (string.IsNullOrEmpty(redirectUri))
                RedirectUri = GetAbsoluteUri(context.Request);
            else
                RedirectUri = redirectUri;
        }

        private string GetAbsoluteUri(HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        public bool GetWxToken(out string token)
        {
            bool res = true;
            //CookieHelper cookie = new CookieHelper(Context);
            //token = cookie.GetValue(AppSecret + "Token");
            string sql = string.Format("select Token from wx_token where Name='{0}' and ValidityTime > NOW()",AppName);
            object obj = SqlHelper.Scalar(sql);
            if (obj == null)
            {
                res = GetWxTokenFromWx(out token);
            }
            else
                token = obj.ToString();
            return res;
        }

        private bool GetWxTokenFromWx(out string token)
        {
            string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}",
               CorpId, AppSecret);

            string res = HttpHelper.Get(url);
            Dictionary<string, object> dict = Json.ToObject<Dictionary<string, object>>(res);
            if (!dict.Keys.Contains("access_token"))
            {
                token = res;
                return false;
            }
            else
            {
                //CookieHelper cookie = new CookieHelper(Context);
                int expires_in = Convert.ToInt32(dict["expires_in"]);
                //cookie.AddCookie(AppSecret + "Token", dict["access_token"].ToString(), DateTime.Now.AddSeconds(expires_in));
                token = dict["access_token"].ToString();
                Dictionary<string, string> dictNew = new Dictionary<string, string>();
                dictNew.Add("Name", AppName);
                dictNew.Add("Token", token);
                dictNew.Add("ValidityTime", DateTime.Now.AddSeconds(expires_in).ToString("G"));
                string sql = string.Format("delete from wx_token where Name='{0}'\r\n;", AppName);
                sql += SqlHelper.GetInsertString(dictNew, "wx_token");
                SqlResult eRes = new SqlResult(SqlHelper.Exce(sql));
                if (eRes.IsAllSuccess)
                {
                    return true;
                }
                else
                {
                    token = Json.ToJson(eRes.ErrList);
                    return false;
                }
            }
        }

        private string GotoGetCode()
        {
            string randomString = "1234567890123456";
//            string randomString = Encrypt.GetRandomCodeN(16);
            //Context.Session["randomString"] = randomString;
//            Context.Session.SetString("randomString", randomString);
            //
            //string urlForGettingCode = string.Format(@"https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid={0}" +
            //        "&agentid={1}&redirect_uri={2}&state={3}",
            //        Corpid, AgentId, HttpUtility.UrlEncode(RedirectUri), randomString);
            string urlForGettingCode = string.Format(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}"
                + "&redirect_uri={1}&response_type=code&scope=snsapi_base&agentid={2}&state={3}#wechat_redirect"
                , CorpId, HttpUtility.UrlEncode(RedirectUri), AgentId, randomString);
            Context.Response.Redirect(urlForGettingCode, false);
            return urlForGettingCode;
        }

        private string GetWxUserId(string code, string token)
        {
            string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}",
                token, code);
            string res = HttpHelper.Get(url);
            Dictionary<string, object> dict = Json.ToObject<Dictionary<string, object>>(res);
            if (!dict.Keys.Contains("UserId"))
            {
                return res;
            }
            else
                return dict["UserId"].ToString();
        }

        private Response<User> GetUserInfo()
        {
            Response<User> res = new Response<User>();
            string WxToken = "";

            string code = Context.Request.Query["code"];
            string state = Context.Request.Query["state"];
//            string randomString = Context.Session.GetString("randomString");
            string randomString = "1234567890123456";
            string UserId = "";
            //randomString和state用来防止csrf攻击（跨站请求伪造攻击）
            if (string.IsNullOrEmpty(code)
                || string.IsNullOrEmpty(randomString)
                || !string.Equals(state, randomString))
            {
                res.code = 2;//跳转url至获取code页面
                res.message = GotoGetCode();
                return res;
            }
            else
            {
                if (!GetWxToken(out WxToken))//获取企业微信token失败
                {
                    res.code = 1;
                    res.message = WxToken;
                    return res;
                }
                UserId = GetWxUserId(code, WxToken);
                if (string.IsNullOrEmpty(UserId) || UserId.Contains("errcode"))
                {
                    res.code = 3;//读取userid报错
                    res.message = UserId;
                    return res;
                }
                else
                {
                    string token = Encrypt.GetRandomCodeN(64);
                    SqlResult sRes = UpdateUserToken(UserId, token);
                    if(sRes.IsAllSuccess)
                    {
                        res = GetUserInfoById(UserId);
                    }
                    else
                    {
                        res.code = 4;
                        res.message = sRes.ErrList.ToJson();
                    }
                }
                //cookie.AddCookie("UserId", UserId, DateTime.Now.AddDays(UserInfoSaveCookieDays));
            }
            return res;
        }

        public SqlResult UpdateUserToken(string userId,string token)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Token", token);
            dict.Add("LastLoginTime", DateTime.Now.AddDays(UserInfoSaveCookieDays).ToString("G"));
            string sql = SqlHelper.GetUpdateString(dict, "user", string.Format(" where WechatUserId='{0}'", userId));
            return new SqlResult(SqlHelper.Exce(sql));
        }

        public Response<User> GetUserInfoById(string Id)
        {
            Response<User> res = new Response<User>();
            string sql = string.Format("select * from user where WechatUserId='{0}'", Id);
            string msg = "";
            DataSet ds = SqlHelper.Find(sql, ref msg);
            if (ds == null)
            {
                res.code = 100;
                res.message = msg;
            }
            else
            {
                if (ds.Tables[0].Rows.Count == 0)
                {
                    res.code = 2;
                    res.message = "UserId错误！";
                }
                else
                {
                    res.Result = ModelHelper<User>.FillModel(ds.Tables[0].Rows[0]);
                }
            }
            return res;
        }

        public Response<User> GetUserInfoByToken(string token)
        {
            Response<User> res = new Response<User>();
            string sql = string.Format("select * from user where Token='{0}' and LastLoginTime > NOW()", token);
            string msg = "";
            DataSet ds = SqlHelper.Find(sql, ref msg);
            if (ds == null)
            {
                res.code = 100;
                res.message = msg;
            }
            else
            {
                if (ds.Tables[0].Rows.Count == 0)
                {
                    res.code = 2;
                    res.message = "token错误或过有效期！";
                }
                else
                {
                    res.Result = ModelHelper<User>.FillModel(ds.Tables[0].Rows[0]);
                }
            }
            return res;
        }

        /// <summary>
        /// 用于验证用户是否登录并同时获取当前用户信息
        /// </summary>
        /// <returns>错误代码
        /// 200 成功
        /// 1   非法请求
        /// 2   token错误或过有效期
        /// 100 数据库报错
        /// </returns>
        public Response<User> CheckAndGetUserInfo()
        {
            Response<User> res = new Response<User>();
            //User user = Json.ToObject<User>(Context.Session.GetString("user"));
            string token = Context.Request.Query["token"];
            if (token == null)
            {
                res.code = 1;
                res.message = "非法请求！";
            }
            else
            {
                //Response<User> uRes = GetUserInfoByToken(token);
                //if (uRes.code == 100)//数据库报错
                //{
                //    res.code = 100;
                //    res.message = uRes.message;
                //}
                //else if (uRes.code == 2)//token错误或过有效期！
                //{
                //    res = GetUserInfo();
                //}
                //else//成功获取用户信息
                //{
                //    res = uRes;
                //}
                res = GetUserInfoByToken(token);
            }

            return res;
        }
        
        public string SendWxMsg(string paraJson)
        {
            GetWxToken(out var token);
            var url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + token);
        
            return HttpHelper.Post(url,paraJson);
        }

        public string GetJsonAndSendWxMsg(string ids, string description, string url, string agentId)
        {
            var jObject = new JObject {{"touser", ids}, {"msgtype", "textcard"}, {"agentid", agentId}};
            var innerJObject = new JObject {{"title", "审批通知"}, {"description", description}, {"url", url}};
            jObject.Add("textcard", innerJObject);

            return SendWxMsg(jObject.ToString());
        }

        public Response<string> GetToken()
        {
            Response<string> res = new Response<string>();
            Response<User> uRes = GetUserInfo();
            User user = uRes.Result;
            res.code = uRes.code;
            res.message = uRes.message;
            if(uRes.code==200)
            {
                JObject token = new JObject();
                token.Add("token", user.Token);
                res.Result = token.ToString();
            }
            return res;
        }
    }
}
