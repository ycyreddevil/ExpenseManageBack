using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManageBack.Model
{
    [Table("user_department")]
    public class UserDepartment
    {
        public int Id { get; set; }
        public string WechatUserId { get; set; }
        public int DepartmentId { get; set; }
        public int IsLeader { get; set; }
        public long Order { get; set; }

        public UserDepartment()
        {
            Id = 0;
            WechatUserId = string.Empty;
            DepartmentId = 0;
            IsLeader = 0;
            Order = 0;
        }
    }
}