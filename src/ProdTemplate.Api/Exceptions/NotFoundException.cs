namespace ProdTemplate.Api.Exceptions;

public class NotFoundException(string target) : Exception($"Item not found: {target}");