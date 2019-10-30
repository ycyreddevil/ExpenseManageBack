using System;
using System.Collections.Generic;
using ExpenseManageBack.CustomModel;
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

        public TravelApplyController(TravelApplyService travelApplyService, IHttpContextAccessor accessor)
        {
            _service = travelApplyService;
            _accessor = accessor;
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
                resp.Result = _service.addOrDraft(travelApply.ToObject<TravelApply>(), department, approver.ToObject<JArray>());
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
                resp.Result = _service.getApprover(flowId, token, department);
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