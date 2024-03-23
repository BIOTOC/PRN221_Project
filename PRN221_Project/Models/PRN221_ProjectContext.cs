using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PRN221_Project.Models
{
    public partial class PRN221_ProjectContext : DbContext
    {
        public PRN221_ProjectContext()
        {
        }

        public PRN221_ProjectContext(DbContextOptions<PRN221_ProjectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Calendar> Calendars { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Console.WriteLine(Directory.GetCurrentDirectory());
                IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
                var strConn = config["ConnectionStrings:MyConStr"];
                optionsBuilder.UseSqlServer(strConn);


            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calendar>(entity =>
            {
                entity.ToTable("Calendar");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Class)
                    .HasMaxLength(50)
                    .HasColumnName("class");

                entity.Property(e => e.Room)
                    .HasMaxLength(50)
                    .HasColumnName("room");

                entity.Property(e => e.Subject)
                    .HasMaxLength(50)
                    .HasColumnName("subject");

                entity.Property(e => e.Teacher)
                    .HasMaxLength(50)
                    .HasColumnName("teacher");

                entity.Property(e => e.TimeSlot)
                    .HasMaxLength(50)
                    .HasColumnName("timeSlot");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
