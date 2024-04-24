using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using CustomerFeedbackManagementSystem.Areas.Identity.Data;
using CustomerFeedbackManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CustomerFeedbackManagementSystem.Data
{
    public class CustomerFeedbackManagementSystemContext : IdentityDbContext<CustomerFeedbackManagementSystemUser>
    {
        public CustomerFeedbackManagementSystemContext(DbContextOptions<CustomerFeedbackManagementSystemContext> options)
            : base(options)
        {
        }

        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<FeedbackReceiver> FeedbackReceiver { get; set; }
        public DbSet<Category> Category { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FeedbackReceiver>().HasData(new FeedbackReceiver { FeedbackReceiverID = 1, Name = "Mac Donalds" });
            builder.Entity<FeedbackReceiver>().HasData(new FeedbackReceiver { FeedbackReceiverID = 2, Name = "Burger King" });
            builder.Entity<FeedbackReceiver>().HasData(new FeedbackReceiver { FeedbackReceiverID = 3, Name = "KFC" });

            builder.Entity<Category>().HasData(new Category() { CategoryId = 1, Name = "Praise"});
            builder.Entity<Category>().HasData(new Category() { CategoryId = 2, Name = "Complain" });
            builder.Entity<Category>().HasData(new Category() { CategoryId = 3, Name = "Improvement" });

            var userId = Guid.NewGuid().ToString();

            builder.Entity<CustomerFeedbackManagementSystemUser>().HasData(new CustomerFeedbackManagementSystemUser()
            {
                Id = userId,
                Email = "testuser@test.com",
                NormalizedEmail = "TESTUSER@TESTUSER.COM",
                UserName = "TestUser",
                NormalizedUserName = "TESTUSER",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            });


            builder.Entity<Feedback>().HasData(new Feedback() { 
                FeedbackId = 1,
                FeedbackReceiverID = 1,
                UserId = userId, 
                CategoryId = 1,
                Description = "McDonald's consistently delivers delicious, affordable meals with a diverse menu that satisfies cravings and offers quick service—a go-to for convenient and tasty fast food.",
                SubmissionDate = DateTime.UtcNow
            });

            builder.Entity<Feedback>().HasData(new Feedback()
            {
                FeedbackId = 2,
                FeedbackReceiverID = 2,
                UserId = userId,
                CategoryId = 2,
                Description = "Burger King's inconsistent food quality, slow service, and lackluster customer experience make it a disappointing choice for those seeking a satisfying fast food meal.",
                SubmissionDate = DateTime.UtcNow
            });

            builder.Entity<Feedback>().HasData(new Feedback()
            {
                FeedbackId = 3,
                FeedbackReceiverID = 3,
                UserId = userId,
                CategoryId = 3,
                Description = "While KFC's signature fried chicken remains enticing, its overall dining experience could benefit from faster service and more attention to maintaining cleanliness standards.",
                SubmissionDate = DateTime.UtcNow.AddDays(-30)
            });
        }
    }
}
