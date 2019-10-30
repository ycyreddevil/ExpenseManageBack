using System;
using System.Collections.Generic;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Infrastructure;
using ExpenseManageBack.Model;
using ExpenseManageBack.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManageBack.Model
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

        [HttpGet]
        public Response<string> GetToken()
        {
            Response<string> res = new Response<string>();
            WxHelper wx = new WxHelper(_accessor.HttpContext);
            res = wx.GetToken();
            if(res.code==2)
            {
                Redirect(res.message);
            }
            return res;
        }
    }
}