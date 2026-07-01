using Asp.Versioning;
using Hrs.Api.Business.HotelBusiness;
using Hrs.Api.Repository.HotelRepository;
using Hrs.Api.Shared;
using Hrs.Api.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrs.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/hotel")]
public class HotelsController(IHotelBusiness hotelBusiness, IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    [Route("get-hotel-list")]
    //[Authorize(Policy = "hotel.view")]
    public async Task<IActionResult> GetList()
    {
        var listDetails = await hotelBusiness.GetHotelsAsync();
        return Ok(listDetails);
    }
    
    [HttpGet]
    [Route("get-hotel-details/{hotelId}")]
    [Authorize(Policy = "hotel.view")]
    public async Task<IActionResult> GetDetails([FromQuery] int hotelId)
    {
        var itemDetails = await hotelBusiness.GetHotelByIdAsync(hotelId);
        return Ok(itemDetails);
    }
    
    [HttpPost]
    [Route("create-hotel")]
    [Authorize(Policy = "hotel.create")]
    public async Task<IActionResult> GetDetails([FromBody] CreateHotelDto createHotelDto)
    {
        var createResponse = await hotelBusiness.CreateHotelAsync(createHotelDto);
        return Ok(createResponse);
    }
    
    [HttpPut]
    [Route("update-hotel")]
    [Authorize(Policy = "hotel.create")]
    public async Task<IActionResult> GetDetails([FromBody] UpdateHotelDto updateHotelDto)
    {
        var updateResponse = await hotelBusiness.UpdateHotelAsync(updateHotelDto);
        return Ok(updateResponse);
    }
}