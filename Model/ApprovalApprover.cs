using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManageBack.Model
{
    [Table("approval_approver")]
    public class ApprovalApprover
    {
        public int Id { get; set; }
        public string DocCode { get; set; }
        public string DocumentTableName { get; set; }
        public int Level { get; set; }
        public string WechatUserId { get; set; }

        public ApprovalApprover()
        {
            Id = 0;
            DocCode = string.Empty;
            DocumentTableName = string.Empty;
            Level = 0;
            WechatUserId = string.Empty;
        }
    }
}