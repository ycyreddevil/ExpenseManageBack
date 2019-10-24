using System.ComponentModel.DataAnnotations.Schema;


namespace ExpenseManageBack.Model
{
    [Table("client")]
    public class Client
    {
        public Client()
        {
            Id = 0;
            Code = string.Empty;
            Name = string.Empty;
            Address = string.Empty;
            Contactor = string.Empty;
            ContactorPhone = string.Empty;
            Logo = string.Empty;
            Type = string.Empty;
            Province = string.Empty;
            City = string.Empty;
            Area = string.Empty;
        }
        #region Model
        private int _id;
        private string _code;
        private string _name;
        private string _address;
        private string _contactor;
        private string _contactorphone;
        private string _logo;
        private string _type;
        private string _province;
        private string _city;
        private string _area;
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
        public string Code
        {
            set { _code = value; }
            get { return _code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Address
        {
            set { _address = value; }
            get { return _address; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Contactor
        {
            set { _contactor = value; }
            get { return _contactor; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ContactorPhone
        {
            set { _contactorphone = value; }
            get { return _contactorphone; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Logo
        {
            set { _logo = value; }
            get { return _logo; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            set { _type = value; }
            get { return _type; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Province
        {
            set { _province = value; }
            get { return _province; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string City
        {
            set { _city = value; }
            get { return _city; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Area
        {
            set { _area = value; }
            get { return _area; }
        }
        #endregion Model
    }
}
