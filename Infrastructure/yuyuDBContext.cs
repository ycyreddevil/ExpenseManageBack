using Microsoft.EntityFrameworkCore;

namespace yuyu.Infrastructure
{
    public class yuyuDBContext : DbContext
    {
        public yuyuDBContext(DbContextOptions<yuyuDBContext> options)
            : base(options)
        { }

//        public virtual DbSet<Shipper> Shippers { get; set; }
    }
}