namespace ExpenseManageBack.CustomModel
{
    public class Response
    {
        public string message { get; set; }
        
        public int code { get; set; }

        public Response()
        {
            message = "success";
            code = 200;
        }
    }

    public class Response<T> : Response
    {
        /// <summary>
        /// 回传的结果
        /// </summary>
        public T Result { get; set; }
    }
}