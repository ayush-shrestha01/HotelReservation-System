using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Hrs.Website.Shared;
using Hrs.Website.Shared.Dtos;

namespace Hrs.Website.Business.HotelBusiness;

public class HotelBusiness(HttpClient httpClient) : IHotelBusiness
{
    public async Task<List<HotelDto>> GetHotelList(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/hotel/get-hotel-list");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<HotelDto>>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result.Data;
            }
            return new List<HotelDto>();
        }
        catch
        {
            return new List<HotelDto>();
        }
    }

    public async Task<SystemResponse> Create(HotelDto hotelDto, string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/hotel/create-hotel");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(hotelDto);

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                if (apiResponse.Success)
                {
                    return new SystemResponse
                    {
                        Success = true,
                        Message = "Hotel created successfully",
                    };
                }
            }
            return new SystemResponse
            {
                Success = false,
                Message = "Hotel creation failed",
            };
        }
        catch
        {
            return new SystemResponse
            {
                Success = false,
                Message = "An error occurred while creating the hotel"
            };
        }
    }
}
