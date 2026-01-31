using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIAP.CloudGames.Pagamentos.Api.Middlewares;

public class ForwardedPrefixMiddleware
{
    private readonly RequestDelegate _next;

    public ForwardedPrefixMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var prefix))
        {
            context.Request.PathBase = prefix.ToString();
        }

        await _next(context);
    }
}