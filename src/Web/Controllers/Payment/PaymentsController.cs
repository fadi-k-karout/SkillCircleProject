using Application.Authorization;
using Application.Common.Interfaces.repos;
using Application.DTOs.Payment;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Web.Controllers.Payment;


[Authorize]
[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;
    
    private readonly IAuthorizationService _authorizationService;
    private const string Admin = RoleName.Admin;
    private const string CanSeeUserPayments = PolicyName.CanSeeUserPayments;
    private const string CanCreateNewPaymentsForUser = PolicyName.CanCreateNewPaymentsForUser;

    public PaymentsController(IPaymentRepository paymentRepository, IAuthorizationService authorizationService)
    {
        _paymentRepository = paymentRepository;
        _authorizationService = authorizationService;
    }
    
    
    
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create a new payment", Description = "Creates a new payment record.")]
    public async Task<IActionResult> CreatePaymentAsync([FromBody] RequestPaymentDto paymentDto)
    {
        var ownerId = paymentDto.UserId.ToString();
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanCreateNewPaymentsForUser);
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        if (paymentDto == null)
        {
            return BadRequest("Payment data is required.");
        }

        var payment = await _paymentRepository.CreatePaymentAsync(paymentDto);
        
        return CreatedAtAction(nameof(GetPaymentsByUserIdAsync), new { userId = paymentDto.UserId }, payment);
    }
    
    [Authorize]
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get payments by user ID", Description = "Retrieves all payments for a specified user.")]
    public async Task<IActionResult> GetPaymentsByUserIdAsync(Guid userId)
    {
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, userId, CanSeeUserPayments);
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        var payments = await _paymentRepository.GetPaymentsByUserIdAsync(userId);
        
        if (payments is null || !payments.Any())
        {
            return NotFound("No payments found for this user.");
        }

        return Ok(payments);
    }
    
    [Authorize(Roles = Admin)]
    [HttpPut("{paymentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Update an existing payment", Description = "Updates payment details by ID.")]
    public async Task<IActionResult> UpdatePaymentAsync(Guid paymentId, [FromBody] UpdatePaymentDto paymentDto)
    {
        if (paymentDto == null)
        {
            return BadRequest("Payment data is required.");
        }

        var updatedPayment = await _paymentRepository.UpdatePaymentAsync(paymentId, paymentDto);
        if (updatedPayment == null)
        {
            return NotFound("Payment not found.");
        }

        return Ok(updatedPayment);
    }
    [Authorize(Roles = Admin)]
    [HttpDelete("{paymentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Delete a payment", Description = "Deletes a payment record by ID.")]
    public async Task<IActionResult> DeletePaymentAsync(Guid paymentId)
    {
        var result = await _paymentRepository.DeletePaymentAsync(paymentId);
        if (!result)
        {
            return NotFound("Payment not found.");
        }

        return NoContent(); // 204 No Content
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("pay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] // Successful operation, no content
    [ProducesResponseType(StatusCodes.Status404NotFound)] // If payment not found
    [SwaggerOperation(Summary = "Pay multiple payments", Description = "Change the status of multiple payments to paid.")]
    public async Task<IActionResult> PayMultiplePaymentsAsync([FromBody] List<Guid> paymentIds)
    {
        
        if (paymentIds == null || !paymentIds.Any())
        {
            return BadRequest("No payment IDs provided.");
        }

      
        var payments = await _paymentRepository.GetByIds(paymentIds);

       
        var missingPayments = paymentIds.Except(payments.Select(p => p.Id)).ToList();
        if (missingPayments.Any())
        {
            return NotFound($"Payments with IDs {string.Join(", ", missingPayments)} not found.");
        }

        // Process each payment
        foreach (var payment in payments)
        {
            payment.Pay(); 
            await _paymentRepository.UpdateAsync(payment); 
        }

       
        await _paymentRepository.SaveChangesAsync();

      
        return NoContent(); // 204 No Content
    }

}
