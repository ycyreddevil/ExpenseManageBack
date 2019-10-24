using System;
using System.Collections.Generic;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Model;
using ExpenseManageBack.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManageBack.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FlowController : ControllerBase
    {
        private FlowService _service;

        public FlowController(FlowService flowService)
        {
            _service = flowService;
        }

        /// <summary>
        /// 搜索用户列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<string>> getUserList(string keyword)
        {
            var resp = new Response<List<string>>();

            try
            {
                resp.Result = _service.getUserList(keyword);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 搜索部门列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<string>> getDepartmentList(string keyword)
        {
            var resp = new Response<List<string>>();

            try
            {
                resp.Result = _service.getDepartmentList(keyword);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 搜索岗位列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<string>> getPostList(string keyword)
        {
            var resp = new Response<List<string>>();

            try
            {
                resp.Result = _service.getPostList(keyword);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }

        /// <summary>
        /// 获取各个单据的审批流列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<List<Flow>> getFlowList(string type)
        {
            var resp = new Response<List<Flow>>();

            try
            {
                resp.Result = _service.getFlowList(type);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 启用或禁用单据审批流
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        [HttpPost]
        public Response updateFlowStatus(int flowId)
        {
            var resp = new Response();

            try
            {
                _service.updateFlowStatus(flowId);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }
        
        /// <summary>
        /// 删除单据审批流
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        [HttpPost]
        public Response deleteFlow(int flowId)
        {
            var resp = new Response();

            try
            {
                _service.deleteFlow(flowId);
            }
            catch (Exception e)
            {
                resp.code = 500;
                resp.message = e.Message;
            }
            
            return resp;
        }

        /// <summary>
        /// 审批流新增或更新
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        [HttpPost]
        public Response addOrUpdateFlow(Flow flow)
        {
            var resp = new Response();

            try
            {
                _service.addOrUpdate(flow);
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