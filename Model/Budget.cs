using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ExpenseManageBack.Model
{
    [Table("budget")]
    public class Budget
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string FeeDetail { get; set; }
        public double BudgetAmount { get; set; }
        public int ParentId { get; set; }
        public DateTime CreateTime { get; set; }

        public Budget()
        {
            Id = 0;
            DepartmentId = 0;
            DepartmentName = string.Empty;
            FeeDetail = string.Empty;
            BudgetAmount = 0.0;
            ParentId = 0;
            CreateTime = DateTime.Now;
        }
    }
}