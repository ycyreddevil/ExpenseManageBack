using System;
using System.Collections.Generic;
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
    public class ClientController : ControllerBase
    {
        private ClientService serv;
        private User user;
        private IHttpContextAccessor _accessor;

        public ClientController(ClientService clientService, IHttpContextAccessor accessor)
        {
            serv = clientService;
            _accessor = accessor;
            WxHelper wx = new WxHelper("app1", _accessor.HttpContext);
            Response<User> res = wx.CheckAndGetUserInfo();
            if (res.code == 200)
                user = res.Result;
            else//直接返回错误信息！
            {
                Redirect(res.message);
            }
        }

        [HttpGet]
        public Response<User> GetList(string token)
        {
            var res = new Response<User>();

            try
            {
                res.Result = user;
            }
            catch (Exception e)
            {
                res.code = 500;
                res.message = e.Message;
            }

            return res;
        }
    }
}