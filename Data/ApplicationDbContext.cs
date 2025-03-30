using ManagerApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Car> Cars => Set<Car>();

    public DbSet<Service> Services => Set<Service>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

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
    }


}