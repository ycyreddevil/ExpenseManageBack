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
    public class MemberManageController : ControllerBase
    {
        //private ClientService serv;
        private Response<User> userInfo;
        private IHttpContextAccessor _accessor;
        private WxHelper wx;

        public MemberManageController(IHttpContextAccessor accessor)
        {
            //serv = clientService;
            _accessor = accessor;
            wx = new WxHelper(_accessor.HttpContext);
            userInfo = wx.CheckAndGetUserInfo();
        }

        [HttpGet]
        public Response<string> GetDepartment(int id=-1)
        {
            var res = new Response<string>();
            if (userInfo.code != 200)
            {
                res.code = userInfo.code;
                res.message = userInfo.message;
                return res;
            }
            return wx.GetDepartmentList(id);
        }
    }
}