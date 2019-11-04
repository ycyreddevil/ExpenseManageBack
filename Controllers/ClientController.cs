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
        }

        [HttpGet]
        public Response<Dictionary<string, object>> GetList()
        {
            var res = new Response<Dictionary<string, object>>();
            if(userInfo.code!=200)
            {
                res.code = userInfo.code;
                res.message = userInfo.message; 
                return res;
            }
            try
            {
                //CrudHelper<Client> crud = new CrudHelper<Client>("client");
                res.Result = serv.GetList();
            }
            catch (Exception e)
            {
                res.code = 500;
                res.message = e.Message;
            }

            return res;
        }

        [HttpPost]
        public Response<List<Client>> GetByName(string name)
        {
            var res = new Response<List<Client>>();
            if (userInfo.code != 200)
            {
                res.code = userInfo.code;
                res.message = userInfo.message;
                return res;
            }
            try
            {
                CrudHelper<Client> crud = new CrudHelper<Client>("client");
                res.Result = crud.GetList(string.Format("Name like '%{0}%'", name));
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