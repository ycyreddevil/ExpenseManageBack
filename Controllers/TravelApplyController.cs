using System;
using System.Collections.Generic;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Infrastructure;
using ExpenseManageBack.Model;
using ExpenseManageBack.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using yuyu.Infrastructure;

namespace ExpenseManageBack.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TravelApplyController : ControllerBase
    {
        private TravelApplyService _service;
        private IHttpContextAccessor _accessor;
        private Response<User> userInfo;

        public TravelApplyController(TravelApplyService travelApplyService, IHttpContextAccessor accessor)
        {
            _service = travelApplyService;
            _accessor = accessor;
            WxHelper wx = new WxHelper(_accessor.HttpContext);
            userInfo = wx.CheckAndGetUserInfo();
            if (userInfo.code == 2)
                Redirect(userInfo.message);
        }
        
        /// <summary>
        /// 差旅申请 新增或保存草稿
        /// </summary>
        /// <param name="travelApply"></param>
        /// <returns></returns>
        [HttpGet]
        public Response<TravelApply> addOrDraft(string travelApply, string department, string approver)
        {
            var resp = new Response<TravelApply>();

            try
            {
                var wechatUserId = userInfo.Result.WechatUserId;
                resp.Result = _service.addOrDraft(travelApply.ToObject<TravelApply>(), department, approver.ToObject<JArray>(), wechatUserId);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 获取差旅申请可选择审批流
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Response<List<Flow>> getFlow()
        {
            var resp = new Response<List<Flow>>();

            try
            {
                resp.Result = _service.getFlow();
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }

        /// <summary>
        /// 差旅申请 通过审批流获取对应的审批人
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="token"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<JArray> getApprover(int flowId, string token, string department)
        {
            var resp = new Response<JArray>();

            try
            {
                resp.Result = _service.getApprover(flowId, userInfo.Result, department);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 获取我已提交 差旅申请
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<TravelApply>> mySubmitted(int year, int month, string key)
        {
            var resp = new Response<List<TravelApply>>();

            try
            {
                resp.Result = _service.mySubmitted(userInfo.Result.WechatUserId, year, month, key);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 获取我已审批 差旅申请
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<TravelApply>> myApproval(int year, int month, string key)
        {
            var resp = new Response<List<TravelApply>>();

            try
            {
                resp.Result = _service.myApproval(userInfo.Result.WechatUserId, year, month, key);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 获取待我审批 差旅申请
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<TravelApply>> myNotApproval(int year, int month, string key)
        {
            var resp = new Response<List<TravelApply>>();

            try
            {
                resp.Result = _service.myNotApproval(userInfo.Result.WechatUserId, year, month, key);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 获取草稿 差旅申请
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<TravelApply>> myDraft(int year, int month, string key)
        {
            var resp = new Response<List<TravelApply>>();

            try
            {
                resp.Result = _service.myDraft(userInfo.Result.WechatUserId, year, month, key);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="docCode"></param>
        /// <param name="result"></param>
        /// <param name="opinion"></param>
        /// <returns></returns>
        [HttpPost]
        public Response approval(string docCode, string result, string opinion)
        {
            var resp = new Response();

            try
            {
                _service.approval(docCode, userInfo.Result.WechatUserId, result, opinion);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
    }
}