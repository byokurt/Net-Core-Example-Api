using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCoreExampleApi.Filters;

public class ProblemDetailsExceptionFilter : IExceptionFilter, IOrderedFilter
{
    public int Order { get; } = int.MaxValue - 10;

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ProblemDetailsException exception)
        {
            context.Result = new ObjectResult(exception.Value)
            {
                StatusCode = exception.Value.Status,
            };

            context.ExceptionHandled = true;
        }
    }
}