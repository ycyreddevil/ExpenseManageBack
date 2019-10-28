using System;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ExpenseManageBack.Model
{
    public class TravelApply
    {
        public int Id { get; set; }
        public string DocCode { get; set; }
        public DateTime Date { get; set; }
        public string Transportation { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public int NumberOfDays { get; set; }
        public double Miles { get; set; }
        public double TicketFee { get; set; }
        public double AccomodationFee { get; set; }
        public double TollFee { get; set; }
        public double TravelAllowance { get; set; }
        public double TotalMoney { get; set; }
        public string Remark { get; set; }
        public string UserName { get; set; }
        public string WechatUserId { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Status { get; set; }
        public int Level { get; set; }
        public DateTime CreateTime { get; set; }

        public TravelApply()
        {
            Id = 0;
            DocCode = string.Empty;
            Date = DateTime.Now;
            Transportation = string.Empty;
            Departure = string.Empty;
            Destination = string.Empty;
            NumberOfDays = 0;
            Miles = 0.0;
            TicketFee = 0.0;
            AccomodationFee = 0.0;
            TollFee = 0.0;
            TravelAllowance = 0.0;
            TotalMoney = 0.0;
            Remark = string.Empty;
            UserName = string.Empty;
            WechatUserId = string.Empty;
            DepartmentId = string.Empty;
            DepartmentName = string.Empty;
            Status = string.Empty;
            Level = 0;
            CreateTime = DateTime.Now;
        }
    }
}