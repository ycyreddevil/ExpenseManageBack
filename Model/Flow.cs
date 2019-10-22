using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManageBack.Model
{
    [Table("flow")]
    public class Flow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Json { get; set; }
        public string UserId { get; set; }
        public DateTime ModifyTime { get; set; }

        public Flow()
        {
            Id = 0;
            Name = string.Empty;
            Status = string.Empty;
            Type = string.Empty;
            Json = string.Empty;
            UserId = string.Empty;
            ModifyTime = DateTime.Now;
        }
    }
}