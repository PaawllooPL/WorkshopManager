using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Model.DataModels;

namespace WorkshopManager.DAL.EF
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<RepairOrder> RepairOrders { get; set; }
        public DbSet<RepairTask> RepairTasks { get; set; }
        public DbSet<Mechanic> Mechanics { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //fluent api commands
            modelBuilder.Entity<User>()
                .ToTable("AspNetUsers")
                .HasDiscriminator<int>("UserType")
                .HasValue<User>((int)RoleValue.User)
                .HasValue<Owner>((int)RoleValue.Owner)
                .HasValue<Client>((int)RoleValue.Client);

            modelBuilder.Entity<RepairOrder>()
                .Property(r => r.EntryEstimatedCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RepairTask>()
                .Property(r => r.Cost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Role>().HasData(
                new Role((int)RoleValue.User ,Enum.GetName(typeof(RoleValue), (int)RoleValue.User)!, RoleValue.User)
            );
            modelBuilder.Entity<Role>().HasData(
                new Role((int)RoleValue.Owner, Enum.GetName(typeof(RoleValue), (int)RoleValue.Owner)!, RoleValue.Owner)
            );
            modelBuilder.Entity<Role>().HasData(
                new Role((int)RoleValue.Client, Enum.GetName(typeof(RoleValue), (int)RoleValue.Client)!, RoleValue.Client)
            );

        }
    }
}
