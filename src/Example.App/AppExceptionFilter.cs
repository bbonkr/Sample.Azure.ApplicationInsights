using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Example.App;

public class AppExceptionFilter : kr.bbon.AspNetCore.Filters.ApiExceptionHandlerFilter
{
    private readonly ILogger _logger;

    public AppExceptionFilter(ILogger<AppExceptionFilter> logger)
    {
        _logger = logger;
    }

    protected override void HandleException(ExceptionContext context)
    {
        if (context.Exception != null)
        {
            _logger.LogError(context.Exception, "{message}::{@exception}", context.Exception.Message, context.Exception);
        }

        base.HandleException(context);
    }
}
