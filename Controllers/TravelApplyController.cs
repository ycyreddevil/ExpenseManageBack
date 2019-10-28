using System;
using System.Collections.Generic;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Model;
using ExpenseManageBack.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ExpenseManageBack.Controllers
{
    public class TravelApplyController : ControllerBase
    {
        private TravelApplyService _service;

        public TravelApplyController(TravelApplyService travelApplyService)
        {
            _service = travelApplyService;
        }
        
        /// <summary>
        /// 差旅申请 新增或保存草稿
        /// </summary>
        /// <param name="travelApply"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<TravelApply> addOrUpdate(TravelApply travelApply)
        {
            var resp = new Response<TravelApply>();

            try
            {
                resp.Result = _service.addOrUpdate(travelApply);
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
        public Response<JArray> getApprover(int flowId, string token, int departmentId)
        {
            var resp = new Response<JArray>();

            try
            {
                resp.Result = _service.getApprover(flowId, token, departmentId);
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