using Hrs.Api.Shared;
using Hrs.Api.Shared.Dtos;

namespace Hrs.Api.Repository.HotelRepository;

public interface IHotelRepository
{
    Task<SystemResponse> UpdateHotel(UpdateHotelDto updateHotelDto);
}