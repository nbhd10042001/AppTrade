

using App.Models.Contacts;
using App.Models.Product;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Models;

// ke thua cac dataset co san trong IdentityDbContext
public class AppDbContext : IdentityDbContext<AppUser> //DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating (ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //loai bo tien to AspNet o TableName (Identity)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName!= null && tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6)); //bo di 6 ki tu dau tien
            }
        }

        // fluent API cho product
        modelBuilder.Entity<CategoryProductModel>(entity => {
            entity.HasIndex(c => c.Slug) // danh chi muc de sau nay tim kiem nhanh va de dang
                    .IsUnique(); // thiet lap chi muc Slug la duy nhat, khong duoc phep co 2 category co slug giong nhay
        });

        modelBuilder.Entity<ProductModel>(entity => {
            entity.HasIndex(p => p.Slug).IsUnique();
        });

        modelBuilder.Entity<ProductCategoryProduct>(entity => {
            entity.HasKey(c => new {c.ProductID, c.CategoryProductID});
        });
       
    }
    public DbSet<ContactModel> Contacts {set; get;}

    public DbSet<CategoryProductModel> CategoryProducts {set; get;}
    public DbSet<ProductModel> Products {set; get;}
    public DbSet<ProductCategoryProduct> ProductCategoryProducts {set; get;}
    public DbSet<ProductPhotoModel> ProductPhotos {set; get;}
}