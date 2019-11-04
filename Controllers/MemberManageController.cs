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
using yuyu.Infrastructure;

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
            wx = new WxHelper(_accessor.HttpContext, "reimburse");
            userInfo = wx.CheckAndGetUserInfo();
        }

        [HttpGet]
        public Response<string> UpdateDepartment()
        {
            var res = new Response<string>();
            if (userInfo.code != 200)
            {
                res.code = userInfo.code;
                res.message = userInfo.message;
                return res;
            }
            var departRes =  wx.GetDepartmentList();
            if(departRes.code != 200)
            {
                res.code = departRes.code;
                res.message = departRes.message;
                return res;
            }
            List<Dictionary<string, object>> list = Json.ToList<Dictionary<string, object>>(departRes.Result);
            List<Department> listDepart = new List<Department>();
            foreach(var dict in list)
            {
                Department d = new Department();
                d.Id = Convert.ToInt32(dict["id"]);
                d.Name = dict["name"].ToString();
                d.ParentId = Convert.ToInt32(dict["parentid"]);
                d.Order  = Convert.ToInt64(dict["order"]);
                listDepart.Add(d);
            }
            for(int i=0;i<listDepart.Count;i++)
            {
                foreach(Department d in listDepart)
                {
                    if(d.Id == listDepart[i].ParentId)
                    {
                        listDepart[i].ParentName = d.Name;
                        break;
                    }
                }
            }
            try
            {
                CrudHelper<Department> crud = new CrudHelper<Department>("department");
                crud.Delete();
                crud.Add(listDepart);
            }
            catch(Exception e)
            {
                res.code = 500;
                res.message = e.ToString();
            }
            

            return res;
        }
    }
}