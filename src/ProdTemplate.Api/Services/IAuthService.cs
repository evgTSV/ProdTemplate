using ProdTemplate.Api.Models.Dto.Requests;
using ProdTemplate.Api.Models.Dto.Responses;

namespace ProdTemplate.Api.Services;

public interface IAuthService
{
    public Task<AuthResponse> SignIn(SignInRequest request);
    public Task<AuthResponse> SignUp(SignUpRequest request);
}