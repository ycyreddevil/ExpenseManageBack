using System;
using System.Collections.Generic;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Model;
using ExpenseManageBack.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManageBack.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private ClientService serv;

        public ClientController(ClientService clientService)
        {
            serv = clientService;
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