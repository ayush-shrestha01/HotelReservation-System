using Asp.Versioning;
using Hrs.Api.Business.RoomBusiness;
using Hrs.Api.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hrs.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/room")]
public class RoomController(IRoomBusiness roomBusiness) : ControllerBase
{
    [HttpGet]
    [Route("get-room-list")]
    public async Task<IActionResult> GetList()
    {
        var listDetails = await roomBusiness.GetRoomsAsync();
        return Ok(listDetails);
    }
    
    [HttpGet]
    [Route("get-room-details/{roomId}")]
    public async Task<IActionResult> GetDetails(int roomId)
    {
        var itemDetails = await roomBusiness.GetRoomByIdAsync(roomId);
        return Ok(itemDetails);
    }
    
    [HttpPost]
    [Route("create-room")]
    public async Task<IActionResult> GetDetails([FromBody] CreateRoomDto createRoomDto)
    {
        var createResponse = await roomBusiness.CreateRoomAsync(createRoomDto);
        return Ok(createResponse);
    }
    
    [HttpPut]
    [Route("update-room")]
    public async Task<IActionResult> GetDetails([FromBody] UpdateRoomDto updateRoomDto)
    {
        var updateResponse = await roomBusiness.UpdateRoomAsync(updateRoomDto);
        return Ok(updateResponse);
    }
    
    [HttpPatch]
    [Route("update-room-status")]
    public async Task<IActionResult> UpdateRoomStatus([FromBody] UpdateRoomDto updateRoomDto)
    {
        var updateResponse = await roomBusiness.UpdateRoomStatusAsync(updateRoomDto);
        return Ok(updateResponse);
    }
}