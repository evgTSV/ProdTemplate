namespace ProdTemplate.Api.Exceptions;

public class UnauthorizedException(string msg) : Exception(msg);