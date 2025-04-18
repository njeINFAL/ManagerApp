using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    public DbSet<Service> Services => Set<Service>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderService> WorkOrderServicess => Set<WorkOrderService>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ApplicationUser - Car kapcsolat (1:N)
        builder.Entity<Car>()
            .HasOne(c => c.ApplicationUser)
            .WithMany(u => u.Cars)
            .HasForeignKey(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Car - WorkOrder kapcsolat (1:N)
        builder.Entity<WorkOrder>()
            .HasOne(w => w.Car)
            .WithMany(c => c.WorkOrders)
            .HasForeignKey(w => w.CarId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<WorkOrderService>()
              .HasOne(wos => wos.WorkOrder)
              .WithMany(wo => wo.WorkOrderServices)
              .HasForeignKey(wos => wos.WorkOrderId);

        builder.Entity<WorkOrderService>()
            .HasOne(wos => wos.Service)
            .WithMany(s => s.WorkOrderServices)
            .HasForeignKey(wos => wos.ServiceId);

        builder.Entity<WorkOrderService>()
            .HasOne(wos => wos.ResponsibleUser)
            .WithMany()
            .HasForeignKey(wos => wos.ResponsibleUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<WorkOrderService>()
            .Property(wos => wos.Status)
            .HasConversion<string>();

        builder.Entity<WorkOrder>()
           .HasOne(w => w.User)
           .WithMany(u => u.WorkOrders)
           .HasForeignKey(w => w.UserId)
           .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Holiday>().HasData(
            new Holiday { HolidayId = 1, Date = new DateTime(2025, 1, 1) },
            new Holiday { HolidayId = 2, Date = new DateTime(2025, 3, 15) },
            new Holiday { HolidayId = 3, Date = new DateTime(2025, 5, 1) },
            new Holiday { HolidayId = 4, Date = new DateTime(2025, 10, 23), Description = "1956-os forradalom" },
            new Holiday { HolidayId = 5, Date = new DateTime(2025, 11, 1) },
            new Holiday { HolidayId = 6, Date = new DateTime(2025, 12, 25) },
            new Holiday { HolidayId = 7, Date = new DateTime(2025, 12, 26) }
            );
    }


}