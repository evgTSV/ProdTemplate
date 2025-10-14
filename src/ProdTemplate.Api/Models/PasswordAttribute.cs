using System.ComponentModel.DataAnnotations;

namespace ProdTemplate.Api.Models;

public class PasswordAttribute() : RegularExpressionAttribute(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");