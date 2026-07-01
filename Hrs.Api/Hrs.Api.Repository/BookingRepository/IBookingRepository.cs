using Hrs.Api.Repository.Models;

namespace Hrs.Api.Repository.BookingRepository;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetBookingsWithDetailsAsync();
    Task<Booking?> GetBookingWithDetailsByIdAsync(int id);
    Task<IEnumerable<Booking>> GetBookingsByHotelIdAsync(int hotelId);
    Task<Booking?> GetBookingByHotelAndRoomIdAsync(int hotelId, int roomId);
}
