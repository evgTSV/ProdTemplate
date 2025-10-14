using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProdTemplate.Api.Exceptions;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;
        
        switch (context.Exception)
        {
            case UnauthorizedAccessException ex:
                context.Result = new ObjectResult(new { Reason = ex.Message }) { StatusCode = 401 };
                break;
            
            default:
                context.ExceptionHandled = false;
                break;
        }
    }
}