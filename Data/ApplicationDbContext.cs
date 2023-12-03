using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Hubs;
using WebApplication1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ServicesPost> ServicesPosts { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<CategoryPosts> CategoryPosts { get; set; }
        public DbSet<UserImage> UserImage { get; set; }


        public DbSet<WebApplication1.Models.UserRole> UserRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<User>()
                .HasOne(u => u.UserImage)
                .WithOne(i => i.User)
                .HasForeignKey<UserImage>(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                 base.OnModelCreating(modelBuilder);
        }

        public DbSet<WebApplication1.Models.ProfileViewModel> ProfileViewModel { get; set; }
    }

}
