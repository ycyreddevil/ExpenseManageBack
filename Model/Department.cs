using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManageBack.Model
{
    [Table("department")]
    public class Department
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public int Order { get; set; }

        public Department()
        {
            Id = 0;
            ParentId = 0;
            Name = string.Empty;
            ParentName = string.Empty;
            Order = 1;
        }
    }
}