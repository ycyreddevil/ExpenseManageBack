using System;
using System.ComponentModel.DataAnnotations.Schema;
using Remotion.Linq.Parsing.ExpressionVisitors.MemberBindings;

namespace ExpenseManageBack.Model
{
    [Table("user")]
    public class User
    {
        public int Id { get; set; }
        public string WechatUserId { get; set;}
        public string UserName { get; set;}
        public string Mobile { get; set;}
        public string Position { get; set;}
        public string Email { get; set;}
        public string Gender { get; set;}
        public string Avatar { get; set;}
        public string Status { get; set; }
        public string Address { get; set; }
        public string Token { get; set; }
        public DateTime LastLoginTime { get; set; }

        public User()
        {
            Id = 0;
            WechatUserId = string.Empty;
            UserName = string.Empty;
            Mobile = string.Empty;
            Position = string.Empty;
            Email = string.Empty;
            Gender = string.Empty;
            Avatar = string.Empty;
            Status = string.Empty;
            Address = string.Empty;
            Token = string.Empty;
            LastLoginTime = DateTime.Now;
        }
    }
}