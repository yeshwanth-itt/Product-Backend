using Microsoft.EntityFrameworkCore;

namespace Product.Backend.Infrastructure
{
    public class ProductDbContext : DbContext
    {
        public DbSet<Domain.Product> Products { get; set; }

        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                    .UseIdentityColumn(seed: 100000, increment: 1);

                entity.Property(p => p.Stock).IsRequired();

                entity.Property(p => p.Name).HasMaxLength(30);
                
                entity.Property(p => p.Description).HasMaxLength(100);

                entity.Property(p => p.Category).HasMaxLength(30);

                entity.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();

                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");

                entity.ToTable("Products",t =>
                {
                    t.HasCheckConstraint("CK_Product_Stock_NonNegative", "[Stock] >= 0");
                    t.HasCheckConstraint("CK_Product_Price_NonNegative", "[Price] >= 0");
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

