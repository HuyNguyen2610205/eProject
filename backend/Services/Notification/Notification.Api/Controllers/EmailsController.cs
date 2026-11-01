using Microsoft.AspNetCore.Mvc;
using Notification.Api.DTOs.Requests;
using Notification.Api.DTOs.Responses;
using Notification.Api.Services.Interfaces;

namespace Notification.Api.Controllers;

[ApiController]
[Route("api/notifications/[controller]")]
public class EmailsController : ControllerBase
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EmailsController> _logger;

    public EmailsController(IEmailSender emailSender, ILogger<EmailsController> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    [HttpPost("send")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendEmail([FromBody] EmailSendRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Nhận yêu cầu gửi email tới: {ToEmail}, Tiêu đề: {Subject}", request.ToEmail, request.Subject);
        var success = await _emailSender.SendEmailAsync(request, cancellationToken);

        if (!success)
        {
            return BadRequest(ApiResponse<bool>.Fail("Gửi email thất bại. Vui lòng kiểm tra lại cấu hình SMTP hoặc thông tin người nhận."));
        }

        return Ok(ApiResponse<bool>.Ok(true, "Email đã được gửi thành công."));
    }

    [HttpPost("send-verification")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendVerificationEmail([FromBody] SendVerificationEmailRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Nhận yêu cầu gửi email xác thực tới: {Email}", request.Email);
        var success = await _emailSender.SendVerificationEmailAsync(request, cancellationToken);

        if (!success)
        {
            return BadRequest(ApiResponse<bool>.Fail("Gửi email xác thực thất bại. Vui lòng kiểm tra lại cấu hình SMTP."));
        }

        return Ok(ApiResponse<bool>.Ok(true, "Email xác thực đã được gửi thành công."));
    }

    [HttpPost("send-password-reset")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendPasswordResetEmail([FromBody] SendPasswordResetEmailRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Nhận yêu cầu gửi email đặt lại mật khẩu tới: {Email}", request.Email);
        var success = await _emailSender.SendPasswordResetEmailAsync(request, cancellationToken);

        if (!success)
        {
            return BadRequest(ApiResponse<bool>.Fail("Gửi email đặt lại mật khẩu thất bại. Vui lòng kiểm tra lại cấu hình SMTP."));
        }

        return Ok(ApiResponse<bool>.Ok(true, "Email đặt lại mật khẩu đã được gửi thành công."));
    }
}
