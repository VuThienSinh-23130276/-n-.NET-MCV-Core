using Do_An_Net_CuoiKy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Do_An_Net_CuoiKy.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<AppUserModel>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Configure Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrderDetail
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(od => od.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Cart
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Carts)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint: một user chỉ có một cart item cho một product
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
            });

            // Configure Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne(r => r.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint: một user chỉ đánh giá một product một lần
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
            });

            // Seed data (optional - có thể xóa nếu không cần)
        }

        //private void SeedData(ModelBuilder modelBuilder)
        //{
        //    // Seed Categories
        //    modelBuilder.Entity<Category>().HasData(
        //        new Category { Id = 1, Name = "Thức Ăn", Description = "Thức ăn dinh dưỡng cho thú cưng", IsActive = true },
        //        new Category { Id = 2, Name = "Đồ Chơi", Description = "Đồ chơi vui nhộn cho thú cưng", IsActive = true },
        //        new Category { Id = 3, Name = "Phụ Kiện", Description = "Phụ kiện thời trang cho thú cưng", IsActive = true },
        //        new Category { Id = 4, Name = "Chăm Sóc", Description = "Sản phẩm chăm sóc sức khỏe", IsActive = true }
        //    );

        //    // Seed Products
        //    modelBuilder.Entity<Product>().HasData(
        //        new Product
        //        {
        //            Id = 1,
        //            Name = "Thức Ăn Khô Cho Chó",
        //            Description = "Thức ăn khô chất lượng cao, đầy đủ dinh dưỡng cho chó",
        //            Price = 250000,
        //            OldPrice = 300000,
        //            CategoryId = 1,
        //            Stock = 100,
        //            Rating = 4.5m,
        //            ReviewCount = 120,
        //            IsActive = true,
        //            CreatedDate = DateTime.Now
        //        },
        //        new Product
        //        {
        //            Id = 2,
        //            Name = "Đồ Chơi Xương Gặm",
        //            Description = "Xương gặm giúp làm sạch răng và giải trí cho chó",
        //            Price = 150000,
        //            CategoryId = 2,
        //            Stock = 50,
        //            Rating = 4.0m,
        //            ReviewCount = 80,
        //            IsActive = true,
        //            CreatedDate = DateTime.Now
        //        },
        //        new Product
        //        {
        //            Id = 3,
        //            Name = "Vòng Cổ Thời Trang",
        //            Description = "Vòng cổ đẹp mắt, chất liệu cao cấp",
        //            Price = 180000,
        //            CategoryId = 3,
        //            Stock = 75,
        //            Rating = 5.0m,
        //            ReviewCount = 45,
        //            IsActive = true,
        //            CreatedDate = DateTime.Now
        //        },
        //        new Product
        //        {
        //            Id = 4,
        //            Name = "Sữa Tắm Cho Chó",
        //            Description = "Sữa tắm dịu nhẹ, làm sạch và thơm mát",
        //            Price = 120000,
        //            CategoryId = 4,
        //            Stock = 60,
        //            Rating = 4.5m,
        //            ReviewCount = 95,
        //            IsActive = true,
        //            CreatedDate = DateTime.Now
        //        }
        //    );
        //}
    }
}
