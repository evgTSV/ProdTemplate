namespace ProdTemplate.Api.Exceptions;

public class ForbiddenException(string target) : Exception($"Access to {target} is denied.");