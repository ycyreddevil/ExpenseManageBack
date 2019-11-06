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
        public Response UpdateDepartment()
        {
            var res = new Response();
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
            int rootIndex = -1;
            for(int i=0;i<listDepart.Count;i++)
            {
                if (listDepart[i].ParentId == 0)
                {
                    rootIndex = i;
                    listDepart[rootIndex].FullName = listDepart[rootIndex].Name;
                    break;
                }                    
            }

            SetAllSubDepartmentFullNameAndParentName(ref listDepart, rootIndex);

            try
            {
                CrudHelper<Department> crud = new CrudHelper<Department>("department");
                crud.Delete();
                crud.Add(listDepart,true);
            }
            catch(Exception e)
            {
                res.code = 500;
                res.message = e.ToString();
            }
            

            return res;
        }

        private void SetAllSubDepartmentFullNameAndParentName(ref List<Department> list, int index)
        {
            Department parentDepart = list[index];
            //if (parentDepart.ParentId == 0)
            //    return;
            for(int i=0;i<list.Count;i++)
            {
                Department subDepart = list[i];
                if (subDepart.ParentId == parentDepart.Id)
                {
                    subDepart.ParentName = parentDepart.Name;
                    subDepart.FullName = parentDepart.FullName + "/" + subDepart.Name;
                    SetAllSubDepartmentFullNameAndParentName(ref list, i);
                }
            }
        }

        [HttpGet]
        public Response UpdateAllUserInfo()
        {
            var res = new Response();
            if (userInfo.code != 200)
            {
                res.code = userInfo.code;
                res.message = userInfo.message;
                return res;
            }
            var userRes = wx.GetUserList();
            if (userRes.code != 200)
            {
                res.code = userRes.code;
                res.message = userRes.message;
                return res;
            }
            List<Dictionary<string, object>> list = Json.ToList<Dictionary<string, object>>(userRes.Result);
            try
            {
                CrudHelper<User> uCrud = new CrudHelper<User>("user");
                List<User> oldList = uCrud.GetList();
                List<User> newList = new List<User>();
                List<UserDepartment> udList = new List<UserDepartment>();

                for (int i = 0; i < oldList.Count; i++)
                {
                    oldList[i].Status = "2";//将已存如本地数据库，但未在企业微信找到的用户状态置为禁用
                }
                foreach (var dict in list)
                {
                    int index=-1;
                    string WechatUserId = dict["userid"].ToString();
                    for(int i=0;i<oldList.Count;i++)
                    {
                        if(oldList[i].WechatUserId == WechatUserId)
                        {
                            index = i;
                            break;
                        }
                    }
                    if(index>=0)
                    {
                        User u = oldList[index];
                        u.UserName = ToolDictionary.GetValueString("name",dict);
                        u.Address= ToolDictionary.GetValueString("address", dict);
                        u.Avatar =  ToolDictionary.GetValueString("avatar", dict);
                        u.Email = ToolDictionary.GetValueString("email", dict);
                        u.Gender =  ToolDictionary.GetValueString("gender", dict);
                        u.Mobile =  ToolDictionary.GetValueString("mobile", dict);
                        u.Position =  ToolDictionary.GetValueString("position", dict);
                        u.Status =  ToolDictionary.GetValueString("status", dict);
                    }
                    else
                    {
                        User u = new User();
                        u.WechatUserId = WechatUserId;
                        u.UserName = ToolDictionary.GetValueString("name", dict);
                        u.Address = ToolDictionary.GetValueString("address", dict);
                        u.Avatar = ToolDictionary.GetValueString("avatar", dict);
                        u.Email = ToolDictionary.GetValueString("email", dict);
                        u.Gender = ToolDictionary.GetValueString("gender", dict);
                        u.Mobile = ToolDictionary.GetValueString("mobile", dict);
                        u.Position = ToolDictionary.GetValueString("position", dict);
                        u.Status = ToolDictionary.GetValueString("status", dict);
                        newList.Add(u);
                    }

                    //构建UserDepartment数据
                    List<int> dl = Json.ToList<int>(dict["department"].ToString());
                    List<int> ol = Json.ToList<int>(dict["order"].ToString());
                    List<int> ll = Json.ToList<int>(dict["is_leader_in_dept"].ToString());
                    for(int i=0;i<dl.Count;i++)
                    {
                        UserDepartment ud = new UserDepartment();
                        ud.WechatUserId = WechatUserId;
                        ud.DepartmentId = dl[i];
                        ud.IsLeader = ll[i];
                        ud.Order = ol[i];
                        udList.Add(ud);
                    }
                }
                CrudHelper<UserDepartment> udCrud = new CrudHelper<UserDepartment>("user_department");
                uCrud.Update(oldList);
                uCrud.Add(newList);
                udCrud.Delete();
                udCrud.Add(udList);
            }
            catch (Exception e)
            {
                res.code = 500;
                res.message = e.ToString();
            }

            List<Department> listDepart = new List<Department>();
            

            return res;
        }
    }
}