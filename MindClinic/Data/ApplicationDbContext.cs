using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using MindClinic.Models;

namespace MindClinic.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; } // list
        public DbSet<DoctorClass> Doctors { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Awards> Awards { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public  DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<SecretaryClass> Secretary { get; set; }
        public DbSet<SecretaryRequests> SecretaryRequests { get; set; }
        public DbSet<Rating> Ratings { get; set; }




        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
