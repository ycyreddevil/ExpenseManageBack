using System.ComponentModel.DataAnnotations.Schema;


namespace ExpenseManageBack.Model
{
    [Table("client_type")]
    public class ClientType
    {
        public ClientType()
        {
            Id = 0;
            Name = string.Empty;
        }
        #region Model
        private int _id;
        private string _name;
        /// <summary>
        /// auto_increment
        /// </summary>
        public int Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        #endregion Model
    }
}
