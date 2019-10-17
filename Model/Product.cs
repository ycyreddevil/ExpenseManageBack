using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManageBack.Model
{
    [Table("product")]
    public class Product
    {
        public Product()
        {
            Id = 0;
            Code = string.Empty;
            Name = string.Empty;
            Specification = string.Empty;
            Unit = string.Empty;
            Type = string.Empty;
            Manufacture = string.Empty;
            Cost = 0.0;
            Price = 0.0;
            License = string.Empty;
            StorageAndTransportationConditions = string.Empty;
            ProductRegistrationNumber = string.Empty;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Specification { get; set; }
        public string Unit { get; set; }
        public string Type { get; set; }
        public string Manufacture { get; set; }
        public double Cost { get; set; }
        public double Price { get; set; }
        public string License { get; set; }
        public string StorageAndTransportationConditions { get; set; }
        public string ProductRegistrationNumber { get; set; }
        
        
    }
}