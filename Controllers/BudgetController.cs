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
    public class BudgetController : ControllerBase
    {
        private BudgetService _service;
        private Response<User> userInfo;
        private IHttpContextAccessor _accessor;
        private WxHelper wx;
        
        public BudgetController(BudgetService service, IHttpContextAccessor accessor)
        {
            _service = service;
            _accessor = accessor;
            wx = new WxHelper(_accessor.HttpContext);
            userInfo = wx.CheckAndGetUserInfo();            
        }

        /// <summary>
        /// 获取部门父节点预算
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Response<List<Budget>> getParentBudget()
        {
            var res = new Response<List<Budget>>();

            try
            {
                res.Result = _service.getParentBudget();
            }
            catch (Exception ex)
            {
                res.code = 500;
                res.message = ex.Message;
            }
            
            return res;
        }
    }
}