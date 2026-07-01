using Hrs.Api.Shared;
using Hrs.Api.Shared.Dtos;

namespace Hrs.Api.Business.HotelBusiness;

public interface IHotelBusiness
{
    Task<ApiResponse<List<HotelDto>>> GetHotelsAsync();
    Task<ApiResponse<HotelDto>> GetHotelByIdAsync(int id);
    Task<ApiResponse<HotelDto>> CreateHotelAsync(CreateHotelDto hotelDto);
    Task<ApiResponse<HotelDto>> UpdateHotelAsync(UpdateHotelDto hotelDto);
}