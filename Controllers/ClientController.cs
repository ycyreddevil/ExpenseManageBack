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
        private Response<User> userInfo;
        private IHttpContextAccessor _accessor;

        public ClientController(ClientService clientService, IHttpContextAccessor accessor)
        {
            serv = clientService;
            _accessor = accessor;
            WxHelper wx = new WxHelper(_accessor.HttpContext);
            userInfo = wx.CheckAndGetUserInfo();
            if (userInfo.code == 2)
                Redirect(userInfo.message);
            
        }

        [HttpGet]
        public Response<User> GetList(string token)
        {
            var res = new Response<User>();

            try
            {
                res = userInfo;
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