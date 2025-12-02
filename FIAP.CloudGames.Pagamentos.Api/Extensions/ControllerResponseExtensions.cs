using FIAP.CloudGames.Pagamentos.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIAP.CloudGames.Pagamentos.Api.Extensions
{
    public static class ControllerResponseExtensions
    {
        public static IActionResult ApiOk<T>(this ControllerBase controller, T data, string? message = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var response = ApiResponse<T>.Ok(data, message);
            return controller.StatusCode((int)statusCode, response);
        }

        public static IActionResult ApiFail(this ControllerBase controller, string message, List<string>? errors = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var response = ApiResponse<string>.Fail(message, errors);
            return controller.StatusCode((int)statusCode, response);
        }
    }
}
