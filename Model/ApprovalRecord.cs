using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManageBack.Model
{
    [Table("approval_record")]
    public class ApprovalRecord
    {
        public int Id { get; set; }
        public string DocCode { get; set; }
        public string DocumentTableName { get; set; }
        public int Level { get; set; }
        public string WechatUserId { get; set; }
        public DateTime Time { get; set; }
        public string ApprovalResult { get; set; }
        public string ApprovalOpinions { get; set; }

        public ApprovalRecord()
        {
            Id = 0;
            DocCode = string.Empty;
            DocumentTableName = string.Empty;
            Level = 0;
            WechatUserId = string.Empty;
            Time = DateTime.Now;
            ApprovalResult = string.Empty;
            ApprovalOpinions = string.Empty;
        }
    }
}