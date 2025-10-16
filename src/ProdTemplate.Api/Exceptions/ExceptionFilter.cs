using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ProdTemplate.Api.Exceptions;

// ReSharper disable once ClassNeverInstantiated.Global
public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;
        
        switch (context.Exception)
        {
            case UnauthorizedAccessException:
                context.Result = new ObjectResult(new { Reason = context.Exception.Message }) { StatusCode = 401 };
                break;
            
            case DbUpdateException
            {
                InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            }:
                context.Result = new ConflictResult();
                break;
            
            case ForbiddenException:
                context.Result = new ObjectResult(new { Reason = context.Exception.Message }) { StatusCode = 403 };
                break;

            case NotFoundException:
                context.Result = new ObjectResult(new { Reason = context.Exception.Message }) { StatusCode = 404 };
                break;
            
            default:
                context.ExceptionHandled = false;
                break;
        }
    }
}