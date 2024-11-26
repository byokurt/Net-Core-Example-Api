using System;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreExampleApi.Filters;

public class ProblemDetailsException : Exception
{
    public ProblemDetailsException(ProblemDetails value)
    {
        Value = value;
    }

    public ProblemDetails Value { get; }
}