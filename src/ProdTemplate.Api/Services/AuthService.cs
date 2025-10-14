using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProdTemplate.Api.Exceptions;
using ProdTemplate.Api.Models.Dto.Requests;
using ProdTemplate.Api.Models.Dto.Responses;
using ProdTemplate.Api.Models.Entities;

namespace ProdTemplate.Api.Services;

public class AuthService(
    ProdTemplateContext context,
    IJwtService jwtService) : IAuthService
{
    private readonly PasswordHasher<string> _passwordHasher = new();
    
    public async Task<AuthResponse> SignIn(SignInRequest request)
    {
        var user = await context.Users.SingleAsync(u => 
            u.Email == request.UsernameOrEmail || 
            u.Username == request.UsernameOrEmail);

        var result = _passwordHasher.VerifyHashedPassword(user.Email, user.Password, request.Password);
        return result == PasswordVerificationResult.Failed 
            ? throw new UnauthorizedException("Invalid username or password") 
            : new AuthResponse(jwtService.GenerateToken(user));
    }

    public async Task<AuthResponse> SignUp(SignUpRequest request)
    {
        var hash = _passwordHasher.HashPassword(request.Email, request.Password);
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = hash
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new AuthResponse(jwtService.GenerateToken(user));
    }
}