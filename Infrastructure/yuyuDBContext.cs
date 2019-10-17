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
    }
}