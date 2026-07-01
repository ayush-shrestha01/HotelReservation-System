using Hrs.Website.Shared;
using Hrs.Website.Shared.Dtos;

namespace Hrs.Website.Business.HotelBusiness;

public interface IHotelBusiness
{
    Task<List<HotelDto>> GetHotelList(string token);
    Task<SystemResponse> Create(HotelDto hotelDto, string token);
}