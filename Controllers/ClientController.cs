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

        private IHttpContextAccessor _accessor;

        public ClientController(ClientService clientService, IHttpContextAccessor accessor)
        {
            serv = clientService;
            _accessor = accessor;
            //WxHelper wx = new WxHelper("app1", _accessor.HttpContext);

        }

        [HttpPost]
        public Response<Dictionary<string, object>> GetList()
        {
            var res = new Response<Dictionary<string, object>>();

            try
            {
                res.Result = serv.GetList();
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