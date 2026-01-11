using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExamPlatform.API.DTOs;
using ExamPlatform.API.Models;
using ExamPlatform.API.Repositories;

namespace ExamPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;

    public PaymentController(IPaymentRepository paymentRepository, IUserRepository userRepository)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

    [HttpPost("create-order")]
    public async Task<ActionResult<ApiResponse<object>>> CreateOrder([FromBody] decimal amount)
    {
        // Simplified - In production, integrate with Razorpay/Stripe
        var orderId = "order_" + Guid.NewGuid().ToString();
        var payment = new Payment
        {
            UserId = GetUserId(),
            OrderId = orderId,
            Amount = amount,
            Status = "pending"
        };

        await _paymentRepository.CreateAsync(payment);
        return Ok(new ApiResponse<object> { Success = true, Message = "Order created", Data = new { orderId, amount } });
    }

    [HttpPost("verify")]
    public async Task<ActionResult<ApiResponse<object>>> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
        if (payment == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Payment not found" });

        // Simplified - In production, verify with Razorpay/Stripe
        payment.PaymentId = request.PaymentId;
        payment.Status = "success";
        await _paymentRepository.UpdateAsync(payment);

        // Update user subscription
        var user = await _userRepository.GetByIdAsync(payment.UserId);
        if (user != null)
        {
            user.SubscriptionType = "paid";
            await _userRepository.UpdateAsync(user);
        }

        return Ok(new ApiResponse<object> { Success = true, Message = "Payment verified successfully" });
    }

    [HttpGet("my-payments")]
    public async Task<ActionResult<ApiResponse<List<Payment>>>> GetMyPayments()
    {
        var payments = await _paymentRepository.GetByUserIdAsync(GetUserId());
        return Ok(new ApiResponse<List<Payment>> { Success = true, Data = payments });
    }
}

