using System.ComponentModel.DataAnnotations;

namespace ProdTemplate.Api.Models.Dto.Requests;

public class SignUpRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = null!;
    
    /// <example>qwerty@telecom.su</example>
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    /// <example>Qwerty123!</example>
    [Required]
    [Password]
    public string Password { get; set; }
}