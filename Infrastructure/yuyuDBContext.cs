using ExpenseManageBack.Model;
using Microsoft.EntityFrameworkCore;

namespace yuyu.Infrastructure
{
    public class yuyuDBContext : DbContext
    {
        public yuyuDBContext(DbContextOptions<yuyuDBContext> options)
            : base(options)
        { }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Flow> Flows { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientType> ClientTypes { get; set; }
    }
}