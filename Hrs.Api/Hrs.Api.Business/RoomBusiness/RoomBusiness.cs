using Hrs.Api.Repository.Models;
using Hrs.Api.Repository.UnitOfWork;
using Hrs.Api.Shared;
using Hrs.Api.Shared.Dtos;

namespace Hrs.Api.Business.RoomBusiness;

public class RoomBusiness(IUnitOfWork unitOfWork) : IRoomBusiness
{
    public async Task<ApiResponse<List<RoomDto>>> GetRoomsAsync()
    {
        ApiResponse<List<RoomDto>> response = new ApiResponse<List<RoomDto>>();
        var repository = unitOfWork.Repository<Room>();
        var entity = await repository.GetAllAsync();
        if (entity == null)
        {
            response.Success = false;
            response.Message = "No rooms found.";
        }
        else
        {
            var roomDtos = entity.Select(h => new RoomDto()
            {
                Id = h.Id,
                HotelId = h.HotelId,
                RoomNumber = h.RoomNumber,
                Type = h.Type,
                PricePerNight = h.PricePerNight,
                IsAvailable = h.IsAvailable
            }).ToList();  
            
            response.Success = true;
            response.Message = "Rooms retrieved successfully.";
            response.Data = roomDtos;
        }
        return response;
    }

    public async Task<ApiResponse<RoomDto>> GetRoomByIdAsync(int id)
    {
        ApiResponse<RoomDto> response = new ApiResponse<RoomDto>();
        var repository = unitOfWork.Repository<Room>();
        var roomDetails = await repository.GetByIdAsync(id);
        if (roomDetails == null)
        {
            response.Success = false;
        }
        else
        {
            RoomDto roomDto = new RoomDto()
            {
                Id = roomDetails.Id,
                HotelId = roomDetails.HotelId,
                RoomNumber = roomDetails.RoomNumber,
                Type = roomDetails.Type,
                PricePerNight = roomDetails.PricePerNight,
                IsAvailable = roomDetails.IsAvailable
            };
            response.Success = true;
            response.Data = roomDto;
        }
        return response;
    }

    public async Task<ApiResponse<RoomDto>> CreateRoomAsync(CreateRoomDto roomDto)
    {
        ApiResponse<RoomDto> response = new ApiResponse<RoomDto>();
        var repository = unitOfWork.Repository<Room>();
        Room room = new Room()
        {
           HotelId = roomDto.HotelId,
           RoomNumber = roomDto.RoomNumber,
           Type = roomDto.Type,
           PricePerNight = roomDto.PricePerNight,
           IsAvailable = roomDto.IsAvailable
        };
       
        await repository.AddAsync(room);
        await repository.SaveChangesAsync();
        response.Success = true;
        return response;
    }

    public async Task<ApiResponse<RoomDto>> UpdateRoomAsync(UpdateRoomDto roomDto)
    {
        ApiResponse<RoomDto> response = new ApiResponse<RoomDto>();
        var repository = unitOfWork.Repository<Room>();
        var room = await repository.GetByIdAsync(roomDto.Id);
        if (room == null)
        {
            response.Success = false;
            response.Message = "Room not found.";
            return response;
        }

        // Restrict status change if active bookings exist
        var bookingRepo = unitOfWork.Repository<Booking>();
        var bookings = await bookingRepo.GetAllAsync();
        var hasActiveBookings = bookings.Any(b => b.RoomId == roomDto.Id && 
            (b.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) || 
             b.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)));

        if (hasActiveBookings && room.IsAvailable != roomDto.IsAvailable)
        {
            response.Success = false;
            response.Message = "Cannot change availability because this room has active reservations.";
            return response;
        }

        room.HotelId = roomDto.HotelId;
        room.RoomNumber = roomDto.RoomNumber;
        room.Type = roomDto.Type;
        room.PricePerNight = roomDto.PricePerNight;
        room.IsAvailable = roomDto.IsAvailable;

        repository.Update(room);
        await repository.SaveChangesAsync();
        response.Success = true;
        return response;
    }

    public async Task<ApiResponse<RoomDto>> UpdateRoomStatusAsync(UpdateRoomDto updateRoomDto)
    {
        ApiResponse<RoomDto> response = new ApiResponse<RoomDto>();
        var repository = unitOfWork.Repository<Room>();
        var room = await repository.GetByIdAsync(updateRoomDto.Id);
        if (room == null)
        {
            response.Success = false;
            response.Message = "Room not found.";
            return response;
        }

        // Restrict status change if active bookings exist
        var bookingRepo = unitOfWork.Repository<Booking>();
        var bookings = await bookingRepo.GetAllAsync();
        var hasActiveBookings = bookings.Any(b => b.RoomId == updateRoomDto.Id && 
            (b.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) || 
             b.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)));

        if (hasActiveBookings)
        {
            response.Success = false;
            response.Message = "Cannot change availability because this room has active reservations.";
            return response;
        }

        room.IsAvailable = updateRoomDto.IsAvailable;

        repository.Update(room);
        await repository.SaveChangesAsync();
        response.Success = true;
        return response;
    }
}