﻿using System;
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
        private WxHelper wx;

        public TravelApplyController(TravelApplyService travelApplyService, IHttpContextAccessor accessor)
        {
            _service = travelApplyService;
            _accessor = accessor;
            wx = new WxHelper(_accessor.HttpContext, "travelApply");
            userInfo = wx.CheckAndGetUserInfo();
        }
        
        /// <summary>
        /// 差旅申请 新增或保存草稿
        /// </summary>
        /// <param name="travelApply"></param>
        /// <returns></returns>
        [HttpGet]
        public Response<TravelApply> addOrDraft(string travelApply, string approver)
        {
            var resp = new Response<TravelApply>();

            try
            {
                var _userInfo = userInfo.Result;
                resp.Result = _service.addOrDraft(travelApply.ToObject<TravelApply>(), approver.ToObject<JArray>(), _userInfo, wx);
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
        public Response<JArray> getApprover(int flowId, string department)
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
                _service.approval(docCode, userInfo.Result.WechatUserId, result, opinion, wx, userInfo.Result);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }

        /// <summary>
        /// 查询差旅申请单据详情
        /// </summary>
        /// <param name="docCode"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<Dictionary<string, object>> getByCode(string docCode)
        {
            var result = new Response<Dictionary<string, object>>();

            try
            {
                result.Result = _service.getByCode(docCode);
            }
            catch (Exception e)
            {
                result.code = 500;
                result.message = e.Message;
            }

            return result;
        }

        /// <summary>
        /// 差旅申请单 撤回
        /// </summary>
        /// <param name="docCode"></param>
        /// <returns></returns>
        [HttpPost]
        public Response withdrew(string docCode)
        {
            var result = new Response();

            try
            {
                _service.withdrew(docCode, userInfo.Result.WechatUserId);
            }
            catch (Exception e)
            {
                result.code = 500;
                result.message = e.Message;
            }

            return result;
        }

        /// <summary>
        /// 与我相关的差旅申请单
        /// </summary>
        /// <param name="type"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<TravelApplyListModel>> relateToMe(string type, int year, int month, string keyword)
        {
            var result = new Response<List<TravelApplyListModel>>();

            try
            {
                if ("beApproved".Equals(type))
                    result.Result = _service.myNotApproval(userInfo.Result.WechatUserId, year, month, keyword);
                else if ("hasApproved".Equals(type))
                    result.Result = _service.myApproval(userInfo.Result.WechatUserId, year, month, keyword);
                else if ("draftBox".Equals(type))
                    result.Result = _service.myDraft(userInfo.Result.WechatUserId, year, month, keyword, userInfo.Result.UserName);
            }
            catch (Exception e)
            {
                result.code = 500;
                result.message = e.Message;
            }

            return result;
        }
    }
}