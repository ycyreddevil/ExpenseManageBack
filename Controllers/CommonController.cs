using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Infrastructure;
using ExpenseManageBack.Model;
using ExpenseManageBack.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManageBack.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private IHttpContextAccessor _accessor;

        public CommonController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// 请求token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<string> getToken()
        {
            Response<string> res = new Response<string>();
            WxHelper wx = new WxHelper(_accessor.HttpContext);
            res = wx.GetToken();
            //if (res.code == 1000)
            //{
            //    Redirect(res.message);
            //}
            return res;
        }

        [HttpGet]
        public Response setToken(string token)
        {
            Response res = new Response();
            CookieHelper cookie = new CookieHelper(_accessor.HttpContext);
            cookie.DeleteCookie("userToken");
            cookie.AddCookie("userToken", token, DateTime.Now.AddDays(30));
            return res;
        }

        [HttpGet]
        public Response ClearToken()
        {
            Response res = new Response();
            CookieHelper cookie = new CookieHelper(_accessor.HttpContext);
            cookie.DeleteCookie("userToken");
            return res;
        }

        [HttpPost]
        public Response<string> SendWxTextMessage(string text, string _touser = "@all")
        {
            WxHelper wx = new WxHelper(_accessor.HttpContext);
            WxTextMessage msg = new WxTextMessage(Convert.ToInt32(wx.AgentId), text, _touser);
            return wx.SendWxTextMsg(msg);
        }

        [HttpPost]
        public Response<string> SendWxTextCardMessage(string _title, string _description, string _url, string _btntxt, string _touser = "@all")
        {
            WxHelper wx = new WxHelper(_accessor.HttpContext);
            WxTextCardMessage msg = new WxTextCardMessage(Convert.ToInt32(wx.AgentId), _title, _description, _url, _btntxt);
            return wx.SendWxTextCardMessage(msg);
        }
    }
}