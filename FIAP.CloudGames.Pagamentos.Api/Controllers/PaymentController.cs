using FIAP.CloudGames.Pagamentos.Api.Extensions;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;
using FIAP.CloudGames.Pagamentos.Domain.Models;
using FIAP.CloudGames.Pagamentos.Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIAP.CloudGames.Pagamentos.Api.Controllers;

/// <summary>
/// Handles HTTP requests related to payment processing and retrieval operations.
/// </summary>
/// <remarks>All endpoints require authentication unless explicitly marked as allowing anonymous access. The
/// controller provides endpoints for processing payments, retrieving payments by date, and fetching payment details by
/// order ID. Standardized API response types are used for error handling and status reporting.</remarks>
/// <param name="service">The payment service used to process payments and retrieve payment information. Cannot be null.</param>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
public class PaymentController(IPaymentService service) : ControllerBase
{
    /// <summary>
    /// Processes a payment request and returns the result of the payment operation.
    /// </summary>
    /// <param name="request">The payment request details to be processed. Cannot be null.</param>
    /// <returns>An IActionResult containing the result of the payment processing operation. Returns a success response if the
    /// payment is processed successfully; otherwise, returns a failure response with an error message.</returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
    {
        try
        {
            var response = await service.ProcessPaymentAsync(request);
            return this.ApiOk(response, "Pagamento Recebido com sucesso");
        }
        catch (Exception ex)
        {
            return this.ApiFail(ex.Message);
        }

    }

    /// <summary>
    /// Retrieves a list of payments for the specified date.
    /// </summary>
    /// <param name="date">The date for which to retrieve payments. Only payments made on this date are included.</param>
    /// <returns>An <see cref="IActionResult"/> containing the list of payments for the specified date. Returns a response with
    /// status code 204 (No Content) if no payments are found.</returns>
    [HttpGet]
    public async Task<IActionResult> GetPaymentsByDate([FromQuery] DateTime date)
    {
        var payments = await service.GetPaymentsByDateAsync(date);
        if (!payments.Any())
            return this.ApiOk(payments, "Nenhum pagamento listado para a data", HttpStatusCode.NoContent);

        return Ok(payments);
    }

    /// <summary>
    /// Retrieves the payment information associated with the specified order identifier.
    /// </summary>
    /// <remarks>If no payment is found for the given order identifier, the response will indicate no content.
    /// This endpoint is typically used to query payment status or details for an existing order.</remarks>
    /// <param name="orderId">The unique identifier of the order for which to retrieve payment details. Cannot be null.</param>
    /// <returns>An <see cref="IActionResult"/> containing the payment details if found; otherwise, a response indicating that no
    /// payment was found for the specified order.</returns>
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetPaymentByOrderId(string orderId)
    {
        var payment = await service.GetPaymentByOrderIdAsync(orderId);

        if (payment == null)
            return this.ApiOk(payment, "Pagamento Não Encontrado", HttpStatusCode.NoContent);

        return this.ApiOk(payment, "Pagamento Encontrado");
    }
}