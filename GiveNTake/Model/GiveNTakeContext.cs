﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace GiveNTake.Model
{
    public class GiveNTakeContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public GiveNTakeContext(DbContextOptions<GiveNTakeContext> options):
            base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(sub => sub.ParentCategory)
                .WithMany(c => c.Subcategories)
                .IsRequired(false);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Products)
                .IsRequired(true);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Product)
                .WithMany();
            modelBuilder.Entity<Message>()
                .HasOne(m => m.FromUser)
                .WithMany();
            modelBuilder.Entity<Message>().HasOne(m => m.ToUser)
                .WithMany();

            base.OnModelCreating(modelBuilder);
        }
        public void SeedData()
        {
            if(!Categories.Any())
            {
                var appliances = new Category()
                {
                    Name = "Appliances",
                    Subcategories = new List<Category>() { new Category() { Name = "Microwaves" } }
                };
                Categories.Add(appliances);
                SaveChanges();
            }
            if (!Cities.Any())
            {
                Cities.AddRange(
                    new City { Name = "New York" },
                    new City { Name = "Seattle" },
                    new City { Name = "San Francisco" });
                SaveChanges();
            }
            if (!Users.Any())
            {
                Users.AddRange(
                    new User() { UserId = "seller1@seller.com" },
                    new User() { UserId = "seller2@seller.com" },
                    new User() { UserId = "buyer1@buyer.com" },
                    new User() { UserId = "buyer2@buyer2.com" });
                SaveChanges();
            }
        }
    }
    
}
