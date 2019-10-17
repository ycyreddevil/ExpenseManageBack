using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManageBack.CustomModel;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManageBack.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public Response<string> GetUserName(string token, string requestid = "")
        {
            return null;
        }
    }
}