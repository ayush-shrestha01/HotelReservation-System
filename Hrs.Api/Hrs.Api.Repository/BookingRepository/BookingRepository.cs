using Hrs.Api.Repository.Data;
using Hrs.Api.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Hrs.Api.Repository.BookingRepository;

public class BookingRepository(HrsDbContext context) : IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetBookingsWithDetailsAsync()
    {
        return await context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Include(b => b.User)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Booking?> GetBookingWithDetailsByIdAsync(int id)
    {
        return await context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByHotelIdAsync(int hotelId)
    {
        return await context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Include(b => b.User)
            .Where(b => b.Room.HotelId == hotelId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Booking?> GetBookingByHotelAndRoomIdAsync(int hotelId, int roomId)
    {
        return await context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Room.HotelId == hotelId && b.RoomId == roomId);
    }
}
