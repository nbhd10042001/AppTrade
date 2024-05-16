

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

       
       
    }

    
}