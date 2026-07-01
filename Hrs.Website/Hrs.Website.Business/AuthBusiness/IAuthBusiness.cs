using Hrs.Website.Shared.Dtos;

namespace Hrs.Website.Business.AuthBusiness;

public interface IAuthBusiness
{
    Task<AuthResponseDto?> Login(LoginDto loginDto);
}
