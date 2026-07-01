using Hrs.Api.Shared;
using Hrs.Api.Shared.Dtos;

namespace Hrs.Api.Business.BookingBusiness;

public interface IBookingBusiness
{
    Task<ApiResponse<List<BookingDto>>> GetBookingsAsync();
    Task<ApiResponse<BookingDto>> GetBookingByIdAsync(int id);
    Task<ApiResponse<BookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto);
    Task<ApiResponse<BookingDto>> UpdateBookingStatusAsync(int id, string status);
    Task<ApiResponse<List<BookingDto>>> GetBookingsByHotelIdAsync(int hotelId);
    Task<ApiResponse<BookingDto>> GetBookingByHotelAndRoomIdAsync(int hotelId, int roomId);
}
