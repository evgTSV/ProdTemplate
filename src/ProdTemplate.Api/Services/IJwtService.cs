using Microsoft.IdentityModel.Tokens;
using ProdTemplate.Api.Models.Entities;

namespace ProdTemplate.Api.Services;

public interface IJwtService
{
    TokenValidationParameters ValidationParameters { get; }
    string GenerateToken(User user);
    public User GetUserFromToken(string token);
}