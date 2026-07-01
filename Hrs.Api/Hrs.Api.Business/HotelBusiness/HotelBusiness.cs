using Hrs.Api.Repository.HotelRepository;
using Hrs.Api.Repository.Models;
using Hrs.Api.Repository.UnitOfWork;
using Hrs.Api.Shared;
using Hrs.Api.Shared.Dtos;

namespace Hrs.Api.Business.HotelBusiness;

public class HotelBusiness(IUnitOfWork unitOfWork, IHotelRepository hotelRepository) : IHotelBusiness
{
    public async Task<ApiResponse<List<HotelDto>>> GetHotelsAsync()
    {
        ApiResponse<List<HotelDto>> response = new ApiResponse<List<HotelDto>>();
        var repository = unitOfWork.Repository<Hotel>();
        var entity = await repository.GetAllAsync();
        if (entity == null)
        {
            response.Success = false;
            response.Message = "No hotels found.";
        }
        else
        {
            var hotelDtos = entity.Select(h => new HotelDto()
            {
                Id = h.Id,
                Name = h.Name,
                Location = h.Location,
                StarRating = h.StarRating
            }).ToList();  
            
            response.Success = true;
            response.Message = "Hotels retrieved successfully.";
            response.Data = hotelDtos;
        }
        return response;
    }

    public async Task<ApiResponse<HotelDto>> GetHotelByIdAsync(int id)
    {
        ApiResponse<HotelDto> response = new ApiResponse<HotelDto>();
        var repository = unitOfWork.Repository<Hotel>();
        var hotelDetails = await repository.GetByIdAsync(id);
        if (hotelDetails == null)
        {
            response.Success = false;
        }
        else
        {
            HotelDto hotelDto = new HotelDto()
            {
                Id = hotelDetails.Id,
                Name = hotelDetails.Name,
                Location = hotelDetails.Location,
                StarRating = hotelDetails.StarRating
            };
            response.Success = true;
            response.Data = hotelDto;
        }
        return response;
    }

    public async Task<ApiResponse<HotelDto>> CreateHotelAsync(CreateHotelDto hotelDto)
    {
        ApiResponse<HotelDto> response = new ApiResponse<HotelDto>();
        var repository = unitOfWork.Repository<Hotel>();
        Hotel hotel = new Hotel()
        {
            Name = hotelDto.Name,
            Location = hotelDto.Location,
            StarRating = hotelDto.StarRating
        };
        await repository.AddAsync(hotel);
        await repository.SaveChangesAsync();
        response.Success = true;
        return response;
    }

    public async Task<ApiResponse<HotelDto>> UpdateHotelAsync(UpdateHotelDto hotelDto)
    {
        ApiResponse<HotelDto> response = new ApiResponse<HotelDto>();
        var repository = unitOfWork.Repository<Hotel>();
        var hotel = await repository.GetByIdAsync(hotelDto.Id);
        if (hotel == null)
        {
            response.Success = false;
            response.Message = "Hotel not found.";
            return response;
        }
        
        hotel.Name = hotelDto.Name;
        hotel.Location = hotelDto.Location;
        hotel.StarRating = hotelDto.StarRating;
        
        repository.Update(hotel);
        await repository.SaveChangesAsync();
        response.Success = true;
        return response;
        
        // var hotelRepoResponse = await hotelRepository.UpdateHotel(hotelDto);
        // if(hotelRepoResponse.Success)
        // {
        //     return new ApiResponse<HotelDto>()
        //     {
        //         Success = true,
        //         Message = hotelRepoResponse.Message
        //     };
        // }
        // else
        // {
        //     return new ApiResponse<HotelDto>()
        //     {
        //         Success = false,
        //         Message = hotelRepoResponse.Message
        //     };
        // }
    }
}