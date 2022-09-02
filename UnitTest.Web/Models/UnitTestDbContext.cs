using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UnitTest.Web.Models
{
    public partial class UnitTestDbContext : DbContext
    {
        public UnitTestDbContext()
        {
        }

        public UnitTestDbContext(DbContextOptions<UnitTestDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsFixedLength();

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Kalemler" }, new Category { Id = 2, Name = "Defterler" });  
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
