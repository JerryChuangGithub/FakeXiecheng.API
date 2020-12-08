using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FakeXiecheng.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }

        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<LineItem> LineItemss { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            // {
            //     Id = Guid.NewGuid(),
            //     Title = "ceshititle",
            //     Description = "shoming",
            //     OriginalPrice = 0,
            //     CreateTime = DateTime.UtcNow
            // });

            var touristRouteJsonData =
                File.ReadAllText(
                    Path.GetDirectoryName(
                        Assembly.GetExecutingAssembly().Location) +
                    @"/Database/touristRoutesMockData.json");

            IList<TouristRoute> touristRoutes =
                JsonConvert
                    .DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);

            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var touristRoutePictureJsonData =
                File.ReadAllText(
                    Path.GetDirectoryName(
                        Assembly.GetExecutingAssembly().Location) +
                    @"/Database/touristRoutePicturesMockData.json");

            IList<TouristRoutePicture> touristRoutePictures =
                JsonConvert
                    .DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);

            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

            modelBuilder.Entity<ApplicationUser>(u =>
                u.HasMany(x => x.UserRoles)
                    .WithOne().HasForeignKey(ur => ur.UserId).IsRequired());

            const string adminRoleId = "13C30197-CDF5-4541-BBFE-3D29BFAD1DDF";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });

            const string adminUserId = "DE8AE532-9BAA-49E0-A41C-585812AD3A5B";
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@gmail.com",
                NormalizedUserName = "admin@gmail.com".ToUpper(),
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "0912345678",
                PhoneNumberConfirmed = false
            };
            adminUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(adminUser, "Fake123$");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}