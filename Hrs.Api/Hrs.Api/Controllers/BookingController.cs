using Asp.Versioning;
using Hrs.Api.Business.BookingBusiness;
using Hrs.Api.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hrs.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/booking")]
public class BookingController(IBookingBusiness bookingBusiness) : ControllerBase
{
    [HttpGet]
    [Route("get-booking-list")]
    public async Task<IActionResult> GetList()
    {
        var result = await bookingBusiness.GetBookingsAsync();
        return Ok(result);
    }

    [HttpGet]
    [Route("get-booking-details/{bookingId}")]
    public async Task<IActionResult> GetDetails(int bookingId)
    {
        var result = await bookingBusiness.GetBookingByIdAsync(bookingId);
        return Ok(result);
    }

    [HttpPost]
    [Route("create-booking")]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto createBookingDto)
    {
        var result = await bookingBusiness.CreateBookingAsync(createBookingDto);
        return Ok(result);
    }

    [HttpPatch]
    [Route("update-booking-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateBookingStatusDto updateBookingStatusDto)
    {
        var result = await bookingBusiness.UpdateBookingStatusAsync(updateBookingStatusDto.Id, updateBookingStatusDto.Status);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-hotel-bookings/{hotelId}")]
    public async Task<IActionResult> GetHotelBookings(int hotelId)
    {
        var result = await bookingBusiness.GetBookingsByHotelIdAsync(hotelId);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-room-booking/{hotelId}/{roomId}")]
    public async Task<IActionResult> GetRoomBooking(int hotelId, int roomId)
    {
        var result = await bookingBusiness.GetBookingByHotelAndRoomIdAsync(hotelId, roomId);
        return Ok(result);
    }
}
