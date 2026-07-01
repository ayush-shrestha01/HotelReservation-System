using Hrs.Website.Business.HotelBusiness;
using Hrs.Website.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrs.Website.Controllers;

[Authorize]
public class HotelController(IHotelBusiness hotelBusiness) : Controller
{
    [Authorize(Policy = "hotel.view")]
    public async Task<IActionResult> Index()
    {
        var token = User.FindFirst("Token")?.Value ?? string.Empty;
        var hotelList = await hotelBusiness.GetHotelList(token);
        return View(hotelList);
    }

    //[Authorize(Policy = "hotel.create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    //[Authorize(Policy = "hotel.create")]
    public async Task<IActionResult> Create(HotelDto hotelDto)
    {
        var token = User.FindFirst("Token")?.Value ?? string.Empty;
        var response = await hotelBusiness.Create(hotelDto, token);
        if (response.Success)
        {
            TempData["Message"] = "Hotel created successfully.";
        }
        else
        {
            TempData["Message"] = "Hotel creation failed.";
        }
        return RedirectToAction("Index");
    }
}