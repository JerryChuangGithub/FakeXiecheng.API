using System;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }

        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            {
                Id = Guid.NewGuid(),
                Title = "ceshititle",
                Description = "shoming",
                OriginalPrice = 0,
                CreateTime = DateTime.UtcNow
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}