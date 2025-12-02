using FIAP.CloudGames.Pagamentos.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace FIAP.CloudGames.Pagamentos.Api.Filters
{
    public class OwnedGameAccessFilter(ILogger<OwnedGameAccessFilter> logger) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out var authenticatedUserId))
            {
                context.Result = new UnauthorizedObjectResult(
                    ApiResponse<string>.Fail("Authentication required", ["Invalid token or user ID not found."]));

                logger.LogWarning($"Unauthorized access attempt with invalid user ID claim or token.");
                return;
            }

            //if (context.ActionArguments.ContainsKey("request") && context.ActionArguments["request"] is AddOwnedGameRequest addRequest)
            //{
            //    if (authenticatedUserId != addRequest.UserId && roleClaim != "Admin")
            //    {
            //        context.Result = new ObjectResult(
            //            ApiResponse<string>.Fail("Authorization Denied", ["You are not authorized to add games to this user's library."]))
            //        {
            //            StatusCode = (int)HttpStatusCode.Forbidden
            //        };

            //        logger.LogWarning("Authorization denied for user to add game to user {targetUserId}", addRequest.UserId);
            //        return;
            //    }
            //}
            if (context.ActionArguments.ContainsKey("userId") && context.ActionArguments["userId"] is int targetUserId)
            {
                if (authenticatedUserId != targetUserId && roleClaim != "Admin")
                {
                    context.Result = new ObjectResult(
                        ApiResponse<string>.Fail("Authorization Denied", ["You are not authorized to view this user's library."]))
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };

                    logger.LogWarning("Authorization denied for user to access user {targetUserId}'s library", targetUserId);
                    return;
                }
            }
            else
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                logger.LogWarning($"Forbidden access attempt by user, no valid request found.");
                return;
            }

            await next();
        }
    }
}
