using System.Net.Http.Json;
using System.Text.Json;
using Hrs.Website.Shared;
using Hrs.Website.Shared.Dtos;

namespace Hrs.Website.Business.AuthBusiness;

public class AuthBusiness(HttpClient httpClient) : IAuthBusiness
{
    public async Task<AuthResponseDto?> Login(LoginDto loginDto)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Try ApiResponse wrapper first, then direct object
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseDto>>(jsonString, options);
                if (apiResponse?.Success == true && apiResponse.Data != null)
                    return apiResponse.Data;

                var directResponse = JsonSerializer.Deserialize<AuthResponseDto>(jsonString, options);
                if (directResponse != null && !string.IsNullOrEmpty(directResponse.Token))
                    return directResponse;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}
