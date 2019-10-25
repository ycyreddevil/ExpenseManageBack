using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Model;
using Microsoft.AspNetCore.Http;
using yuyu.Infrastructure;

namespace ExpenseManageBack.Infrastructure
{
    public class WxHelper
    {
        private string CorpId = "";
        private string AppSecret = "";
        private string AgentId = "";
        private HttpContext Context;
        public string RedirectUri { get; set; }

        public WxHelper(string appSecret,string agentId,HttpContext context)
        {
            AppSecret = appSecret;
            AgentId = agentId;
            Context = context;
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

        private void GotoGetCode()
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
        }

        public Response CheckAndGetUserInfo()
        {
            Response res = new Response();
            User user = Json.ToObject<User>(Context.Session.GetString("user"));
            if(user==null)
            {
                //string code = Context.Request. ["code"];
                //string state = Context.Request["state"];
                //string randomString = Context.Session.GetString("randomString");
                //string WxToken = GetWxToken();
                //CookieHelper cookie = new CookieHelper(Context);
                //string UserId = CookieHelper.GetCookieValueStatic("UserId");
            }

            return res;
        }
    }
}
