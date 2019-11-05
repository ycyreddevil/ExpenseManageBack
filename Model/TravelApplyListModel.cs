using System;

namespace ExpenseManageBack.Model
{
    public class TravelApplyListModel
    {
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public string Destination { get; set; }
        public string Departure { get; set; }
        public double TotalMoney { get; set; }
        public string DocCode { get; set; }
    }
}