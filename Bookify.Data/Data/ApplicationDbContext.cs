using Bookify.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Bookify.Data.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // RoomType → Rooms (One-to-Many)
            builder.Entity<RoomType>()
                .HasMany(rt => rt.Rooms)
                .WithOne(r => r.RoomType)
                .HasForeignKey(r => r.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room → Bookings (One-to-Many)
            builder.Entity<Room>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser → Bookings (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== التصحيح هنا =====
            // Booking → Payments (One-to-Many) وليس One-to-One
            builder.Entity<Booking>()
                .HasMany(b => b.Payments)
                .WithOne(p => p.Booking)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            // =======================

            // تكوين Decimal Columns
            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            // تكوين String Lengths
            builder.Entity<Booking>()
                .Property(b => b.PaymentStatus)
                .HasMaxLength(50);

            builder.Entity<Booking>()
                .Property(b => b.Status)
                .HasMaxLength(50);

            builder.Entity<Payment>()
                .Property(p => p.PaymentMethod)
                .HasMaxLength(50);

            builder.Entity<Payment>()
                .Property(p => p.Status)
                .HasMaxLength(50);

            // Default Values
            builder.Entity<Booking>()
                .Property(b => b.PaymentStatus)
                .HasDefaultValue("Pending");

            builder.Entity<Booking>()
                .Property(b => b.Status)
                .HasDefaultValue("Pending");

            builder.Entity<Booking>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Payment>()
                .Property(p => p.Status)
                .HasDefaultValue("Pending");

            builder.Entity<Payment>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}