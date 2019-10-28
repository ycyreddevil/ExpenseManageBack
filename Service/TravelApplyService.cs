using System.Collections.Generic;
using System.Linq;
using ExpenseManageBack.Model;
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
        public TravelApply addOrUpdate(TravelApply travelApply)
        {
            if (travelApply.Id == 0)
            {
                travelApply.Status = "已提交";
                _unitWork.Add(travelApply);
            }
            else
            {
                _unitWork.Update(travelApply);
            }

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
        public JArray getApprover(int flowId, string token, int departmentId)
        {
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
                        // 如果是或签 则index相同 如果是会签 则index不同
                        if (process["mode"].ToString() == "or")
                        {
                            approverJObject = new JObject
                            {
                                ["name"] = leader.UserName,
                                ["mobile"] = leader.Mobile,
                                ["level"] = level
                            };
                            level++;
                            if (checkApproverIfExist(result, approverJObject))
                                result.Add(approverJObject);
                        }
                        else
                        {
                            approverJObject = new JObject
                            {
                                ["name"] = leader.UserName,
                                ["mobile"] = leader.Mobile,
                                ["level"] = level ++
                            };
                            if (checkApproverIfExist(result, approverJObject))
                                result.Add(approverJObject);
                        }
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
    }
}