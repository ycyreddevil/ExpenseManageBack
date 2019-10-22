using System.Collections.Generic;
using System.Linq;
using ExpenseManageBack.Model;
using Remotion.Linq.Parsing.ExpressionVisitors.MemberBindings;
using yuyu.Infrastructure;
using yuyu.Service;

namespace ExpenseManageBack.Service
{
    public class FlowService : BaseService
    {
        private IUnitWork _unitWork { get; set; }
        
        public FlowService(IUnitWork unitWork) : base(unitWork)
        {
            _unitWork = unitWork;
        }

        /// <summary>
        /// 搜索用户列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public List<string> getUserList(string keyWord)
        {
            return _unitWork.Find<User>(u => u.UserName.Contains(keyWord)).Select(u => u.UserName).ToList();
        }

        /// <summary>
        /// 搜索部门列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public List<string> getDepartmentList(string keyWord)
        {
            return _unitWork.Find<Department>(u => u.Name.Contains(keyWord)).Select(u => u.Name).ToList();
        }

        /// <summary>
        /// 搜索岗位列表
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public List<string> getPostList(string keyWord)
        {
            return _unitWork.Find<User>(u => u.Position.Contains(keyWord)).Select(u => u.Position).ToList();
        }

        /// <summary>
        /// 获取各个单据的审批流列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Flow> getFlowList(string type)
        {
            return _unitWork.Find<Flow>(u => u.Type.Equals(type)).ToList();
        }

        /// <summary>
        /// 启用或禁用单据审批流
        /// </summary>
        /// <param name="flowId"></param>
        public void updateFlowStatus(int flowId)
        {
            var flow = _unitWork.FindSingle<Flow>(u => u.Id == flowId);

            flow.Status = flow.Status == "启用" ? "禁用" : "启用";
            
            _unitWork.Update(flow);
            _unitWork.Save();
        }

        /// <summary>
        /// 删除单据审批流
        /// </summary>
        /// <param name="flowId"></param>
        public void deleteFlow(int flowId)
        {
            _unitWork.Delete<Flow>(u => u.Id == flowId);
            _unitWork.Save();
        }
    }
}