using Microsoft.AspNetCore.Mvc;
using ProdTemplate.Api.Models.Dto.Requests;
using ProdTemplate.Api.Models.Dto.Responses;
using ProdTemplate.Api.Services;

namespace ProdTemplate.Api.Controllers;

[Controller]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("sign-up")]
    public Task<AuthResponse> SignUp([FromBody] SignUpRequest request) => authService.SignUp(request);

    [HttpPost("sign-in")]
    public Task<AuthResponse> SignIn([FromBody] SignInRequest request) => authService.SignIn(request);
}