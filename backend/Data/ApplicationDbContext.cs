using backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<MechanicAvailability> MechanicAvailabilities => Set<MechanicAvailability>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderService> WorkOrderServicess => Set<WorkOrderService>();
    public DbSet<PartsCategory> PartsCategories => Set<PartsCategory>();
    public DbSet<PartItem> PartItems => Set<PartItem>();
    public DbSet<PartOrder> PartOrders => Set<PartOrder>();
    public DbSet<PartOrderItem> PartOrderItems => Set<PartOrderItem>();


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

        // WorkOrder for Client and Mechanic Role
        builder.Entity<WorkOrder>()
           .HasOne(w => w.Client)
           .WithMany(u => u.ClientWorkOrders)
           .HasForeignKey(w => w.ClientId)
           .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<WorkOrder>()
           .HasOne(w => w.Mechanic)
           .WithMany(u => u.MechanicWorkOrders)
           .HasForeignKey(w => w.MechanicId)
           .OnDelete(DeleteBehavior.NoAction);

        //ApplicationUser - MechanicAvailability kapcsolat 
        builder.Entity<MechanicAvailability>()
            .HasOne(ma => ma.ApplicationUser)
            .WithMany(u => u.MechanicAvailabilities)
            .HasForeignKey(ma => ma.ApplicationUserId)
            .OnDelete(DeleteBehavior.SetNull);


        builder.Entity<PartItem>()
            .HasOne(pi => pi.Category)
            .WithMany(pc => pc.PartItems)
            .HasForeignKey(pi => pi.PartsCategoryId);

        builder.Entity<PartOrderItem>()
            .HasOne(poi => poi.PartOrder)
            .WithMany(po => po.PartOrderItems)
            .HasForeignKey(poi => poi.PartOrderId);

        builder.Entity<PartOrderItem>()
            .HasOne(poi => poi.PartItem)
            .WithMany()
            .HasForeignKey(poi => poi.PartItemId);

        // non-changing holiday days
        builder.Entity<Holiday>().HasData(
            new Holiday { HolidayId = 1, Date = new DateTime(2025, 1, 1) },
            new Holiday { HolidayId = 2, Date = new DateTime(2025, 3, 15) },
            new Holiday { HolidayId = 3, Date = new DateTime(2025, 5, 1) },
            new Holiday { HolidayId = 4, Date = new DateTime(2025, 10, 23), Description = "1956-os forradalom" },
            new Holiday { HolidayId = 5, Date = new DateTime(2025, 11, 1) },
            new Holiday { HolidayId = 6, Date = new DateTime(2025, 12, 25) },
            new Holiday { HolidayId = 7, Date = new DateTime(2025, 12, 26) }
            );


        // DEMO PARTS DATA

        builder.Entity<PartsCategory>().HasData(
        new PartsCategory { PartsCategoryId = 1, PartsCategoryName = "Fékrendszer" }
        );

        builder.Entity<PartItem>().HasData(
        new PartItem { PartItemId = 1, PartItemName = "Fék tárcsa", PartsCategoryId = 1 },
        new PartItem { PartItemId = 2, PartItemName = "Fék dob", PartsCategoryId = 1 },
        new PartItem { PartItemId = 3, PartItemName = "Első fékpofa", PartsCategoryId = 1 },
        new PartItem { PartItemId = 4, PartItemName = "Hátsó fékpofa", PartsCategoryId = 1 },
        new PartItem { PartItemId = 5, PartItemName = "ABS gyűrű", PartsCategoryId = 1 },
        new PartItem { PartItemId = 6, PartItemName = "Első fékbetét", PartsCategoryId = 1 },
        new PartItem { PartItemId = 7, PartItemName = "Hátső fékbetét", PartsCategoryId = 1 },
        new PartItem { PartItemId = 8, PartItemName = "Fékkar", PartsCategoryId = 1 },
        new PartItem { PartItemId = 9, PartItemName = "Féktárcsa csavar", PartsCategoryId = 1 },
        new PartItem { PartItemId = 10, PartItemName = "Féklopás jelző", PartsCategoryId = 1 },
        new PartItem { PartItemId = 11, PartItemName = "Komplett fékrendszer", PartsCategoryId = 1 }
        );

        //DEMO WORKORDER

        builder.Entity<WorkOrder>().HasData(
        new WorkOrder
        {
            WorkOrderId = 1,
            AppointmentTime = new DateTime(2025, 4, 29, 14, 0, 0),
            CreatedAt = new DateTime(2025, 4, 25, 16, 30, 0),
            IsActive = true,
            Notes = null,
            CarId = null,
            ClientId = null,
            MechanicId = null, 
        },

        new WorkOrder
        {
            WorkOrderId = 2,
            AppointmentTime = new DateTime(2025, 4, 29, 13, 0, 0),
            CreatedAt = new DateTime(2025, 4, 25, 16, 30, 10),
            IsActive = false,
            Notes = "TÖRÖLVE",
            CarId = null,
            ClientId = null,
            MechanicId = null, 
        },

        new WorkOrder
        {
            WorkOrderId = 3,
            AppointmentTime = new DateTime(2025, 4, 30, 10, 0, 0),
            CreatedAt = new DateTime(2025, 4, 25, 16, 30, 0),
            IsActive = true,
            Notes = null,
            CarId = null,
            ClientId = null,
            MechanicId = null,
        }
       
        );
    }
}