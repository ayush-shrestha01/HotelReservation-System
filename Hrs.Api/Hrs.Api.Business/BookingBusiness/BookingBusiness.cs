using Hrs.Api.Repository.BookingRepository;
using Hrs.Api.Repository.Models;
using Hrs.Api.Repository.UnitOfWork;
using Hrs.Api.Shared;
using Hrs.Api.Shared.Dtos;

namespace Hrs.Api.Business.BookingBusiness;

public class BookingBusiness(IUnitOfWork unitOfWork, IBookingRepository bookingRepository) : IBookingBusiness
{
    public async Task<ApiResponse<List<BookingDto>>> GetBookingsAsync()
    {
        var response = new ApiResponse<List<BookingDto>>();
        var bookings = await bookingRepository.GetBookingsWithDetailsAsync();
        
        response.Data = bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            RoomId = b.RoomId,
            RoomNumber = b.Room?.RoomNumber ?? string.Empty,
            HotelName = b.Room?.Hotel?.Name ?? string.Empty,
            CheckIn = b.CheckIn,
            CheckOut = b.CheckOut,
            TotalPrice = b.TotalPrice,
            Status = b.Status
        }).ToList();
        response.Success = true;
        response.Message = "Bookings retrieved successfully.";
        return response;
    }

    public async Task<ApiResponse<BookingDto>> GetBookingByIdAsync(int id)
    {
        var response = new ApiResponse<BookingDto>();
        var booking = await bookingRepository.GetBookingWithDetailsByIdAsync(id);
        if (booking == null)
        {
            response.Success = false;
            response.Message = "Booking not found.";
            return response;
        }

        response.Data = new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room?.RoomNumber ?? string.Empty,
            HotelName = booking.Room?.Hotel?.Name ?? string.Empty,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status
        };
        response.Success = true;
        return response;
    }

    public async Task<ApiResponse<BookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto)
    {
        var response = new ApiResponse<BookingDto>();
        
        if (createBookingDto.CheckOut <= createBookingDto.CheckIn)
        {
            response.Success = false;
            response.Message = "Check-out date must be after check-in date.";
            return response;
        }

        try
        {
            await unitOfWork.BeginTransactionAsync();

            var roomRepo = unitOfWork.Repository<Room>();
            var room = await roomRepo.GetByIdAsync(createBookingDto.RoomId);
            if (room == null)
            {
                response.Success = false;
                response.Message = "Room not found.";
                return response;
            }

            if (!room.IsAvailable)
            {
                response.Success = false;
                response.Message = "Room is not available for booking.";
                return response;
            }

            // Get a valid user
            var userRepo = unitOfWork.Repository<User>();
            var user = await userRepo.GetByIdAsync(1);
            if (user == null)
            {
                var allUsers = await userRepo.GetAllAsync();
                user = allUsers.FirstOrDefault();
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "No users found in the system to assign the booking.";
                    return response;
                }
            }

            int days = (createBookingDto.CheckOut - createBookingDto.CheckIn).Days;
            if (days <= 0) days = 1;
            decimal totalPrice = room.PricePerNight * days;

            var booking = new Booking
            {
                RoomId = createBookingDto.RoomId,
                UserId = user.Id,
                CheckIn = createBookingDto.CheckIn,
                CheckOut = createBookingDto.CheckOut,
                TotalPrice = totalPrice,
                Status = "Confirmed"
            };

            await unitOfWork.Repository<Booking>().AddAsync(booking);
            
            // Mark room as unavailable
            room.IsAvailable = false;
            roomRepo.Update(room);

            await unitOfWork.CommitAsync();

            response.Success = true;
            response.Message = "Booking created successfully.";
            
            // Fetch newly created booking with details for response
            var createdBooking = await bookingRepository.GetBookingWithDetailsByIdAsync(booking.Id);
            if (createdBooking != null)
            {
                response.Data = new BookingDto
                {
                    Id = createdBooking.Id,
                    RoomId = createdBooking.RoomId,
                    RoomNumber = createdBooking.Room?.RoomNumber ?? string.Empty,
                    HotelName = createdBooking.Room?.Hotel?.Name ?? string.Empty,
                    CheckIn = createdBooking.CheckIn,
                    CheckOut = createdBooking.CheckOut,
                    TotalPrice = createdBooking.TotalPrice,
                    Status = createdBooking.Status
                };
            }
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            response.Success = false;
            response.Message = $"An error occurred while creating booking: {ex.Message}";
        }
        finally
        {
            await unitOfWork.DisposeAsync();
        }

        return response;
    }

    public async Task<ApiResponse<BookingDto>> UpdateBookingStatusAsync(int id, string status)
    {
        var response = new ApiResponse<BookingDto>();
        var bookingRepo = unitOfWork.Repository<Booking>();
        var booking = await bookingRepo.GetByIdAsync(id);
        if (booking == null)
        {
            response.Success = false;
            response.Message = "Booking not found.";
            return response;
        }

        booking.Status = status;
        
        // If booking is cancelled, make room available again
        if (string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            var roomRepo = unitOfWork.Repository<Room>();
            var room = await roomRepo.GetByIdAsync(booking.RoomId);
            if (room != null)
            {
                room.IsAvailable = true;
                roomRepo.Update(room);
            }
        }

        bookingRepo.Update(booking);
        await bookingRepo.SaveChangesAsync();

        // Get details for response
        var updatedBooking = await bookingRepository.GetBookingWithDetailsByIdAsync(id);
        if (updatedBooking != null)
        {
            response.Data = new BookingDto
            {
                Id = updatedBooking.Id,
                RoomId = updatedBooking.RoomId,
                RoomNumber = updatedBooking.Room?.RoomNumber ?? string.Empty,
                HotelName = updatedBooking.Room?.Hotel?.Name ?? string.Empty,
                CheckIn = updatedBooking.CheckIn,
                CheckOut = updatedBooking.CheckOut,
                TotalPrice = updatedBooking.TotalPrice,
                Status = updatedBooking.Status
            };
        }
        response.Success = true;
        response.Message = "Booking status updated successfully.";
        return response;
    }

    public async Task<ApiResponse<List<BookingDto>>> GetBookingsByHotelIdAsync(int hotelId)
    {
        var response = new ApiResponse<List<BookingDto>>();
        var bookings = await bookingRepository.GetBookingsByHotelIdAsync(hotelId);
        response.Data = bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            RoomId = b.RoomId,
            RoomNumber = b.Room?.RoomNumber ?? string.Empty,
            HotelName = b.Room?.Hotel?.Name ?? string.Empty,
            CheckIn = b.CheckIn,
            CheckOut = b.CheckOut,
            TotalPrice = b.TotalPrice,
            Status = b.Status
        }).ToList();
        response.Success = true;
        response.Message = "Bookings for hotel retrieved successfully.";
        return response;
    }

    public async Task<ApiResponse<BookingDto>> GetBookingByHotelAndRoomIdAsync(int hotelId, int roomId)
    {
        var response = new ApiResponse<BookingDto>();
        var booking = await bookingRepository.GetBookingByHotelAndRoomIdAsync(hotelId, roomId);
        if (booking == null)
        {
            response.Success = false;
            response.Message = "No booking found for the specified hotel and room ID.";
            return response;
        }

        response.Data = new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room?.RoomNumber ?? string.Empty,
            HotelName = booking.Room?.Hotel?.Name ?? string.Empty,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status
        };
        response.Success = true;
        return response;
    }
}
