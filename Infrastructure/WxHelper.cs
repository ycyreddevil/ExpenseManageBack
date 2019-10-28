using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using yuyu.Infrastructure;
using ExpenseManageBack.Service;

namespace ExpenseManageBack.Infrastructure
{
    public class WxHelper 
    {
        private string CorpId = "";
        private string AppSecret = "";
        private string AgentId = "";
        private HttpContext Context;
        private int UserInfoSaveCookieDays = 30;
        public string RedirectUri { get; set; }
        private WxHelperService serv = null;

        public WxHelper(string name,HttpContext context, string redirectUri = "")
        {
            WxParameter wxP = new WxParameter("app1");
            AppSecret = wxP.App.Secret;
            AgentId = wxP.App.AgentId;
            CorpId = wxP.CorpId;
            UserInfoSaveCookieDays = wxP.UserInfoSaveCookieDays;
            Context = context;
            if (string.IsNullOrEmpty(redirectUri))
                RedirectUri = GetAbsoluteUri(context.Request);
            else
                RedirectUri = redirectUri;
            serv = new WxHelperService();
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

        public bool GetToken(out string token)
        {
            bool res = true;
            CookieHelper cookie = new CookieHelper(Context);
            token = cookie.GetValue(AppSecret + "Token");
            if(string.IsNullOrEmpty(token))
            {
                res = GetTokenFromWx(out token);
            }
            return res;
        }

        private bool GetTokenFromWx(out string token)
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
                CookieHelper cookie = new CookieHelper(Context);
                int expires_in = Convert.ToInt32(dict["expires_in"]);
                cookie.AddCookie(AppSecret + "Token", dict["access_token"].ToString(), DateTime.Now.AddSeconds(expires_in));
                token = dict["access_token"].ToString();
                return true;
            }
        }

        private string GotoGetCode()
        {
            string randomString = Encrypt.GetRandomCodeN(16);
            //Context.Session["randomString"] = randomString;
            Context.Session.SetString("randomString", randomString);
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

        /// <summary>
        /// 用于验证用户是否登录并同时获取当前用户信息
        /// </summary>
        /// <returns></returns>
        public Response CheckAndGetUserInfo()
        {
            Response res = new Response();
            User user = Json.ToObject<User>(Context.Session.GetString("user"));
            if(user==null)
            {
                string WxToken = "";
                if(!GetToken(out WxToken))//获取企业微信token失败
                {
                    res.code = 1;
                    res.message = WxToken;
                    return res;
                }
                string code = Context.Request.Query["code"];
                string state = Context.Request.Query["state"];
                string randomString = Context.Session.GetString("randomString");
                
                CookieHelper cookie = new CookieHelper(Context);
                string UserId = cookie.GetValue("UserId");
                if(string.IsNullOrEmpty(UserId))
                {
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
                        UserId = GetWxUserId(code, WxToken);
                        if(string.IsNullOrEmpty(UserId) || UserId.Contains("errcode"))
                        {
                            res.code = 3;//读取userid报错
                            res.message = UserId;
                            return res;
                        }
                        else
                            cookie.AddCookie("UserId", UserId, DateTime.Now.AddDays(UserInfoSaveCookieDays));
                    }
                }

                res.message = UserId;
            }

            return res;
        }
    }
}
