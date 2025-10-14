using System.ComponentModel.DataAnnotations;

namespace ProdTemplate.Api.Models.Dto.Requests;

public class SignInRequest
{
    [Required] 
    public string UsernameOrEmail { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}