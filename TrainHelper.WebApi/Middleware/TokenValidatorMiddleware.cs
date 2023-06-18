using TrainHelper.DAL.Providers;
using TrainHelper.WebApi.Constants;

namespace TrainHelper.WebApi.Middleware;

public class TokenValidatorMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidatorMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IUserDataProvider userDataProvider)
    {
        var flag = true;
        var sessionIdString = context.User.Claims.FirstOrDefault(c => c.Type == ClaimNames.SessionId)?.Value;
        if (int.TryParse(sessionIdString, out var sessionId))
        {
            var session = await userDataProvider.GetUserSessionById(sessionId);
            if (session == null || !session.IsActive)
            {
                flag = false;
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        if (flag)
            await _next(context);
    }
}

public static class TokenValidatorMiddlewareExtension
{
    public static IApplicationBuilder UseTokenValidator(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenValidatorMiddleware>();
    }
}