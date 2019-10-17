namespace yuyu.Infrastructure
{
    public class TableData
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int code;
        /// <summary>
        /// 操作消息
        /// </summary>
        public string msg;

        /// <summary>
        /// 总记录条数
        /// </summary>
        public int count;

        /// <summary>
        /// 数据内容
        /// </summary>
        public dynamic data;

        public TableData()
        {
            code = 200;
            msg = "加载成功";
        }
    }
}