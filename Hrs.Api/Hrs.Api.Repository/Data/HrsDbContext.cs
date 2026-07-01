using Hrs.Api.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Hrs.Api.Repository.Data;

public class HrsDbContext(DbContextOptions<HrsDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Name = "Admin",
            Email = "admin@hotel.com",
            PasswordHash = "Admin@123",
            Role = "Admin"
        });

        modelBuilder.Entity<Hotel>().HasData(
            new Hotel { Id = 1, Name = "Grand Plaza", Location = "Kathmandu", StarRating = 5 },
            new Hotel { Id = 2, Name = "Mountain View Inn", Location = "Pokhara", StarRating = 4 }
        );

        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, HotelId = 1, RoomNumber = "101", Type = "Single", PricePerNight = 50, IsAvailable = true },
            new Room { Id = 2, HotelId = 1, RoomNumber = "102", Type = "Double", PricePerNight = 90, IsAvailable = true },
            new Room { Id = 3, HotelId = 1, RoomNumber = "201", Type = "Suite", PricePerNight = 200, IsAvailable = true },
            new Room { Id = 4, HotelId = 2, RoomNumber = "101", Type = "Single", PricePerNight = 40, IsAvailable = true },
            new Room { Id = 5, HotelId = 2, RoomNumber = "102", Type = "Double", PricePerNight = 75, IsAvailable = true }
        );
    }
}
