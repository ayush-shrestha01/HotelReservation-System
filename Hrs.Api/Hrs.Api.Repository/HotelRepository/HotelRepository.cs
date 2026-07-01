using Hrs.Api.Repository.Data;
using Hrs.Api.Shared;
using Hrs.Api.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Hrs.Api.Repository.HotelRepository;

public class HotelRepository(HrsDbContext context) : IHotelRepository
{
    public async Task<SystemResponse> UpdateHotel(UpdateHotelDto updateHotelDto)
    {
        SystemResponse response = new SystemResponse();
        var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == updateHotelDto.Id);
        if (hotel == null)
        {
            response.Success = false;
            response.Message = "Hotel not found.";
            return response;
        }

        hotel.Name = updateHotelDto.Name;
        hotel.Location = updateHotelDto.Location;
        hotel.StarRating = updateHotelDto.StarRating;
        
        await context.SaveChangesAsync();
        
        response.Success = true;
        response.Message = "Hotel updated successfully.";
        return response;
    }
}