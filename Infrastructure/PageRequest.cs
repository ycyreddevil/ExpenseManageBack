namespace ExpenseManageBack.CustomModel
{
    public class PageRequest
    {
        public int page { get; set; }
        public int limit { get; set; }

        public PageRequest()
        {
            page = 1;
            limit = 10;
        }
    }
}