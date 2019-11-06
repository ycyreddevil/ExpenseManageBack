using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManageBack.Model
{
    [Table("department")]
    public class Department
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string ParentName { get; set; }
        public long Order { get; set; }

        public Department()
        {
            Id = 0;
            ParentId = 0;
            Name = string.Empty;
            FullName = string.Empty;
            ParentName = string.Empty;
            Order = 1;
        }
    }
}