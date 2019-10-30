using System.Collections.Generic;
using System.Linq;
using ExpenseManageBack.CustomModel;
using ExpenseManageBack.Infrastructure;
using ExpenseManageBack.Model;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using yuyu.Infrastructure;
using yuyu.Service;

namespace ExpenseManageBack.Service
{
    public class TravelApplyService : BaseService
    {
        private IUnitWork _unitWork { get; set; }

        public TravelApplyService(IUnitWork unitWork) : base(unitWork)
        {
            _unitWork = unitWork;
        }

        /// <summary>
        /// 差旅申请 新增或保存草稿
        /// </summary>
        /// <param name="travelApply"></param>
        /// <returns></returns>
        public TravelApply addOrDraft(TravelApply travelApply, string department, JArray approverJArray, string wechatUserId)
        {
            travelApply.DocCode = Encrypt.GenerateDocCode();
            
            _unitWork.Add(travelApply);
            
            if (travelApply.Status.Equals("已提交"))
            {
                // 新增记录到审批人表
                var approverList = new List<ApprovalApprover>();
                foreach (var jarray in approverJArray)
                {
                    var approver = new ApprovalApprover
                    {
                        DocumentTableName = "差旅申请",
                        DocCode = travelApply.DocCode,
                        Level = int.Parse(jarray["level"].ToString()),
                        WechatUserId = jarray["wechatUserId"].ToString()
                    };
                    approverList.Add(approver);
                };
                _unitWork.BatchAdd(approverList.ToArray());
                
                // 新增记录到审批记录表
                var record = new ApprovalRecord
                {
                    DocumentTableName = "差旅申请",
                    DocCode = travelApply.DocCode,
                    Level = 0,
                    WechatUserId = wechatUserId,
                    ApprovalResult = "单据提交",
                    ApprovalOpinions = "单据提交"
                };
                _unitWork.Add(record);

                // 给下级审批人发消息
                WxHelper wxHelper = new WxHelper(null);    // todo context怎么传
                wxHelper.GetJsonAndSendWxMsg("", "请及时审批单据", "", "");    // todo agentId确定

                // 给提交人发消息
                wxHelper.GetJsonAndSendWxMsg("", "您的单据已提交，请知悉", "", ""); // todo agentId确定
            }

            _unitWork.Save();

            return travelApply;
        }

        /// <summary>
        /// 获取差旅申请可选择审批流
        /// </summary>
        /// <returns></returns>
        public List<Flow> getFlow()
        {
            return _unitWork.Find<Flow>(u => u.Type.Equals("2") && u.Status == "启用").ToList();
        }

        /// <summary>
        /// 通过选择的审批流获取具体的对应人
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public JArray getApprover(int flowId, string token, string department)
        {
            var departmentId = _unitWork.FindSingle<Department>(u => u.Name.Equals(department)).Id;
            
            var processJson = _unitWork.FindSingle<Flow>(u => u.Id == flowId).Json.ToObject<JArray>();
            
            var result = new JArray();

            // 首先第一级是自己
            var user = UnitWork.FindSingle<User>(u => u.Token.Equals(token));
            
            if (user == null)
                return null;
            
            var level = 0;
            var approverJObject = new JObject
            {
                ["name"] = user.UserName,
                ["wechatUserId"] = user.WechatUserId,
                ["level"] = level ++
            };
            result.Add(approverJObject);
            
            foreach (var process in processJson)
            {
                //指定人员审批
                if (process["type"].ToString() == "User")
                {
                    var userName = process["detail"].ToString().ToList<string>()[0];
                    approverJObject = new JObject
                    {
                        ["name"] = userName,
                        ["wechatUserId"] = _unitWork.FindSingle<User>(u => u.UserName.Equals(userName)).WechatUserId,
                        ["level"] = level ++
                    };
                    if (checkApproverIfExist(result, approverJObject))
                        result.Add(approverJObject);
                }
                else if (process["type"].ToString() == "OneSuperior")
                {
                    var leaderList = findDepartmentHeader(user, departmentId);
                    
                    if (user != null) user = null;

                    if (leaderList.Count == 0) continue;

                    departmentId = _unitWork.FindSingle<Department>(u => u.Id == departmentId).ParentId;

                    foreach (var leader in leaderList)
                    {
                        approverJObject = new JObject
                        {
                            ["name"] = leader.UserName,
                            ["wechatUserId"] = leader.WechatUserId,
                            ["level"] = level ++
                        };
                        if (checkApproverIfExist(result, approverJObject))
                            result.Add(approverJObject);
                    }
                }
                else if (process["type"].ToString() == "SuperiorUntil")
                {
                    var finalDepartmentNameList = process["detail"].ToString().ToList<string>();
                    var containFlag = false;
                    var _finalDepartmentName = "";
                    var departmentName = "";
                    foreach (var finalDepartmentName in finalDepartmentNameList)
                    {
                        departmentName = UnitWork.FindSingle<Department>(u => u.Id == departmentId).Name;
                        
                        if (!departmentName.Contains(finalDepartmentName)) 
                            continue;

                        _finalDepartmentName = finalDepartmentName;
                        containFlag = true;
                        break;
                    }

                    if (containFlag)
                    {
                        // 若有关系 则一直往上找
                        while (_finalDepartmentName != departmentName)
                        {
                            var leaderList = findDepartmentHeader(user, departmentId);

                            if (user != null) user = null;
                            
                            if (leaderList == null || leaderList.Count == 0)
                            {
                                continue;
                            }

                            foreach (var leader in leaderList)
                            {
                                approverJObject = new JObject
                                {
                                    ["name"] = leader.UserName,
                                    ["wechatUserId"] = leader.WechatUserId,
                                    ["level"] = level ++
                                };
                                if (checkApproverIfExist(result, approverJObject))
                                    result.Add(approverJObject);
                            }
                            
                            departmentId = UnitWork.FindSingle<Department>(u => u.Id == departmentId).ParentId;
                        }

                        var lastLeaderList = findDepartmentHeader(user, departmentId);

                        if (lastLeaderList == null || lastLeaderList.Count == 0)
                        {
                            continue;
                        }

                        foreach (var leader in lastLeaderList)
                        {
                            approverJObject = new JObject
                            {
                                ["name"] = leader.UserName,
                                ["wechatUserId"] = leader.WechatUserId,
                                ["level"] = level ++
                            };
                            if (checkApproverIfExist(result, approverJObject))
                                result.Add(approverJObject);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 判断找出的审批人是否已经存在
        /// </summary>
        /// <param name="jarray"></param>
        /// <param name="jObject"></param>
        /// <returns></returns>
        private bool checkApproverIfExist(JArray jarray, JObject jObject)
        {
            foreach (JObject _temp in jarray)
            {
                if (_temp["wechatUserId"].ToString().Equals(jObject["wechatUserId"].ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 找到部门所有负责人
        /// </summary>
        /// <param name="user"></param>
        /// <param name="self_departmentId"></param>
        /// <returns></returns>
        private List<User> findDepartmentHeader(User user, int departmentId)
        {
            var departmentUser = _unitWork.FindSingle<UserDepartment>(u =>
                u.DepartmentId == departmentId && u.WechatUserId.Equals(user.WechatUserId));

            // 默认不是领导 则找本部门领导
            var query = from us in _unitWork.Find<User>(null).ToList()
                join ud in _unitWork.Find<UserDepartment>(u =>
                        u.DepartmentId == departmentUser.DepartmentId && u.IsLeader == 1)
                    on us.WechatUserId equals ud.WechatUserId
                select us;
            
            if (departmentUser.IsLeader == 1)
            {
                //如果是领导 则找父部门领导
                var parentDepartmentId = _unitWork.FindSingle<Department>(u => u.Id == departmentUser.DepartmentId).ParentId;
                        
                query = from us in _unitWork.Find<User>(null).ToList()
                    join ud in _unitWork.Find<UserDepartment>(u =>
                            u.DepartmentId == parentDepartmentId && u.IsLeader == 1)
                        on us.WechatUserId equals ud.WechatUserId
                    select us;
            }
            
            return query.ToList();
        }

        /// <summary>
        /// 获取差旅申请单详情
        /// </summary>
        /// <param name="docCode"></param>
        /// <returns></returns>
        public Dictionary<string, object> getDetail(string docCode)
        {
            // 获取单据详情
            var travelApply = _unitWork.FindSingle<TravelApply>(u => u.DocCode.Equals(docCode));
            
            // 获取审批记录
            var query = from approvalRecord in _unitWork
                    .Find<ApprovalRecord>(u => u.DocumentTableName.Equals("差旅申请") && u.DocCode.Equals(docCode)).ToList()
                join user in _unitWork.Find<User>(null).ToList() on approvalRecord.WechatUserId equals user.WechatUserId
                select new Dictionary<string, object>
                {
                    ["level"] = approvalRecord.Level,
                    ["result"] = approvalRecord.ApprovalResult,
                    ["opinion"] = approvalRecord.ApprovalOpinions,
                    ["time"] = approvalRecord.Time,
                    ["userName"] = user.UserName
                };

            var record = query.ToList();
            
            // 获取审批人
            query = from approvalApprover in _unitWork.Find<ApprovalApprover>(u => u.DocumentTableName.Equals("差旅申请") && u.DocCode.Equals(docCode)).ToList()
                join user in _unitWork.Find<User>(null).ToList() on approvalApprover.WechatUserId equals user.WechatUserId
                select new Dictionary<string, object>
                {
                    ["level"] = approvalApprover.Level,
                    ["userName"] = user.UserName
                };
            
            var approver = query.ToList();
            
            var result = new Dictionary<string, object>
            {
                ["travelApply"] = travelApply,
                ["record"] = record,
                ["approver"] = approver
            };
            
            return null;
        }

        /// <summary>
        /// 我已提交的差旅申请单
        /// </summary>
        /// <param name="wechatUserId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TravelApply> mySubmitted(string wechatUserId, int year, int month, string key)
        {
            var iQueryableList = _unitWork.Find<TravelApply>(u => u.WechatUserId.Equals(wechatUserId) && !u.Status.Equals("草稿"));

            if (!string.IsNullOrEmpty(key))
            {
                iQueryableList = iQueryableList.Where(u => u.Departure.Contains(key) || u.Destination.Contains(key));
            }

            if (year != 0 && month != 0)
            {
                iQueryableList = iQueryableList.Where(u => u.Date.Year == year && u.Date.Month == month - 1);
            }

            return iQueryableList.ToList();
        }

        /// <summary>
        /// 我已审批的差旅申请单
        /// </summary>
        /// <param name="wechatUserId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TravelApply> myApproval(string wechatUserId, int year, int month, string key)
        {
            var query = from travelApply in _unitWork.Find<TravelApply>(null).ToList()
                join record in
                    _unitWork.Find<ApprovalRecord>(u =>
                        u.DocumentTableName.Equals("差旅申请") && u.WechatUserId.Equals(wechatUserId))
                    on travelApply.DocCode equals record.DocCode
                select travelApply;
            
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(u => u.Departure.Contains(key) || u.Destination.Contains(key));
            }

            if (year != 0 && month != 0)
            {
                query = query.Where(u => u.Date.Year == year && u.Date.Month == month - 1);
            }
            
            return query.ToList();
        }

        /// <summary>
        /// 待我审批的差旅申请单
        /// </summary>
        /// <param name="wechatUserId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TravelApply> myNotApproval(string wechatUserId, int year, int month, string key)
        {
            var query = from travelApply in _unitWork.Find<TravelApply>(null).ToList()
                join approvalApprover in _unitWork.Find<ApprovalApprover>(
                        u => u.DocumentTableName.Equals("差旅申请") && u.WechatUserId.Equals(wechatUserId)).ToList()
                    on new {u = travelApply.Level, y = travelApply.DocCode} equals new
                    {
                        u = approvalApprover.Level,
                        y = approvalApprover.DocCode
                    }
                select travelApply;
            
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(u => u.Departure.Contains(key) || u.Destination.Contains(key));
            }

            if (year != 0 && month != 0)
            {
                query = query.Where(u => u.Date.Year == year && u.Date.Month == month - 1);
            }

            return query.ToList();
        }

        /// <summary>
        /// 待我提交（草稿状态）的差旅申请单
        /// </summary>
        /// <param name="wechatUserId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<TravelApply> myDraft(string wechatUserId, int year, int month, string key)
        {
            var iQueryableList = _unitWork.Find<TravelApply>(u => u.WechatUserId.Equals(wechatUserId) && u.Status.Equals("草稿"));

            if (!string.IsNullOrEmpty(key))
            {
                iQueryableList = iQueryableList.Where(u => u.Departure.Contains(key) || u.Destination.Contains(key));
            }

            if (year != 0 && month != 0)
            {
                iQueryableList = iQueryableList.Where(u => u.Date.Year == year && u.Date.Month == month - 1);
            }

            return iQueryableList.ToList();
        }

        /// <summary>
        /// 审批差旅申请单
        /// </summary>
        /// <param name="docCode"></param>
        /// <param name="wechatUserId"></param>
        public Response approval(string docCode, string wechatUserId, string result, string opinion)
        {
            var travelApply = _unitWork.FindSingle<TravelApply>(u => u.DocCode.Equals(docCode));
            
            // 首先判断该审批人是否有权限 防止重复点击
            var rightApprover = _unitWork.Find<ApprovalApprover>(u =>
                u.DocCode.Equals(docCode) && u.Level == travelApply.Level && u.WechatUserId.Equals(wechatUserId));
            
            if (rightApprover == null)
            {
                return new Response
                {
                    code = 500,
                    message = "暂无权限审批"
                };
            }

            // 保存审批记录
            var record = new ApprovalRecord
            {
                DocCode = docCode,
                Level = travelApply.Level,
                ApprovalOpinions = opinion,
                ApprovalResult = result,
                WechatUserId = wechatUserId,
                DocumentTableName = "差旅申请"
            };
            _unitWork.Add(record);
            _unitWork.Save();
            
            if ("同意".Equals(result))
            {
                agree(travelApply, docCode);
            }
            else
            {
                reject(travelApply);
            }
            
            _unitWork.Save();
            return null;
        }
        
        /// <summary>
        /// 同意报销单
        /// </summary>
        /// <param name="reimburse"></param>
        /// <param name="docCode"></param>
        private void agree(TravelApply travelApply, string docCode)
        {
            // 判断是否流程结束
            var totalStep = _unitWork.Find<TravelApply>(u => u.DocCode.Equals(docCode)).ToList().Count;

            if (totalStep == travelApply.Level + 1)
            {
                // 流程结束
                travelApply.Status = "已审批";
                _unitWork.Update(travelApply);
                    
                // 发通知给提交人审批流程结束
            
                // 发通知给知悉人
            }
            else
            {
                // 流程没结束
                travelApply.Level = travelApply.Level + 1;
                _unitWork.Update(travelApply);

                // 发通知给提交人 已被审批
                
                // 发通知给知悉人
                
                // 发通知给下一级审批人
            }
        }

        /// <summary>
        /// 驳回报销单
        /// </summary>
        /// <param name="reimburse"></param>
        private void reject(TravelApply travelApply)
        {
            travelApply.Status = "已拒绝";
            travelApply.Level = 0;
            _unitWork.Update(travelApply);
                
            // 发通知给提交人 审批被拒绝

            // 发通知给知悉人
        }

        public void withraw()
        {
            
        }
    }
}