using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ExpenseManageBack.Infrastructure;
using ExpenseManageBack.Model;
using yuyu.Infrastructure;
using yuyu.Service;

namespace ExpenseManageBack.Service
{
    public class BudgetService : BaseService
    {
        private IUnitWork _unitWork { get; set; }

        public BudgetService(IUnitWork unitWork) : base(unitWork)
        {
            _unitWork = unitWork;
        }

        /// <summary>
        /// 获取部门父节点预算
        /// </summary>
        /// <returns></returns>
        public List<Budget> getParentBudget()
        {
            return _unitWork.Find<Budget>(u =>
                u.ParentId == -1 && u.CreateTime.Year == DateTime.Now.Year &&
                u.CreateTime.Month == DateTime.Now.Month).ToList();
        }

        /// <summary>
        /// 获取部门子节点预算
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public List<Budget> getChildrenBudget(int departmentId, int budgetId)
        {
            return _unitWork.Find<Budget>(u =>
                u.DepartmentId == departmentId && u.ParentId == budgetId && u.CreateTime.Year == DateTime.Now.Year &&
                u.CreateTime.Month == DateTime.Now.Month).ToList();
        }

        /// <summary>
        /// 读取excel中的内容进入导入
        /// </summary>
        /// <param name="dt"></param>
        public void importBudget(DataTable dt)
        {
            // 先删除之前的记录
            _unitWork.Delete<Budget>(u => u.CreateTime.Month == DateTime.Now.Month);
            _unitWork.Save();

            // 再新增
            var budgetList = ModelHelper<Budget>.FillModel(dt);
            _unitWork.BatchAdd(budgetList.ToArray());
            _unitWork.Save();
        }

        /// <summary>
        /// 更新某个部门的预算值
        /// </summary>
        /// <param name="budgetAmount"></param>
        /// <param name="budgetId"></param>
        public void updateBudget(double budgetAmount, int budgetId)
        {
            var budget = _unitWork.FindSingle<Budget>(u => u.Id == budgetId);
            budget.BudgetAmount = budgetAmount;

            _unitWork.Update(budget);
            _unitWork.Save();
        }

        public void downTemplate()
        {
            DateTime now = DateTime.Now;
            DateTime last = now.AddMonths(-1);

            var sql = "SELECT import_budget.Id,department.`name`,import_budget.DepartmentId,import_budget.ParentId," +
                      "import_budget.FeeDetail,0 as budget FROM budget as import_budget " +
                      "INNER JOIN department on import_budget.DepartmentId = department.Id " +
                      $"where import_budget.CreateTime between '{last.Year}-{last.Month}-1 00:00:00 ' and '{now.Year}-{now.Month}-1 00:00:00' order by import_budget.Id;";

            DataTable dt = SqlHelper.Find(sql).Tables[0];

            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }

            var listHeadText = new List<string> {"Id", "部门名称", "部门Id", "父节点Id", "费用明细", "预算金额"};

            // todo 把查询内容写入到excel中
        }
    }
}