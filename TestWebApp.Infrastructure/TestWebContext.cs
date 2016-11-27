using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TestWebApp.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TestWebApp.Core.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApp.Infrastructure
{
    public class TestWebContext : IdentityDbContext<ApplicationUser>
    {
        //public DbSet<User> TestUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        //public DbSet<ApplicationUser> Users { get; set; }
        public TestWebContext(DbContextOptions<TestWebContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //foreach (var entity in builder.Model.GetEntityTypes())
            //{
            //    entity.Relational().TableName = entity.ClrType.Name;
            //}
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }


            builder.Entity<ApplicationUser>().ToTable("Users");//.Property(p => p.Id).HasColumnName("ContactId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //builder.Entity<ApplicationRole>().ToTable("Roles");
            //builder.Entity<ContactLogin>().ToTable("ContactLogins");
            //builder.Entity<ContactClaim>().ToTable("ContactClaims").Property(p => p.Id).HasColumnName("ContactClaimId");
            //builder.Entity<Role>().ToTable("Roles").Property(p => p.Id).HasColumnName("RoleId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //builder.Entity<User>()
            //  .ToTable("User");

            builder.Entity<Contact>()
              .ToTable("Contact");

            //builder.Entity<User>()
            //    .Property(u => u.Name)
            //    .HasMaxLength(100)
            //    .IsRequired();

            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
