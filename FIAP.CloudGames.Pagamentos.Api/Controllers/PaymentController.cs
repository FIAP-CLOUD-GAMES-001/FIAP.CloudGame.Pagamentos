using FIAP.CloudGames.Pagamentos.Api.Extensions;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;
using FIAP.CloudGames.Pagamentos.Domain.Models;
using FIAP.CloudGames.Pagamentos.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIAP.CloudGames.Pagamentos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]

    public class PaymentController : ControllerBase
    {

        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return this.ApiOk("request", "request successfully.", HttpStatusCode.Created);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var response = await _service.ProcessPaymentAsync(request);
            return this.ApiOk(response, "Pagamento Recebido com suecsso");
        }
    }
}
