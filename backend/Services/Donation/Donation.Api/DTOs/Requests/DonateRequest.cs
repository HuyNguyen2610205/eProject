using System.ComponentModel.DataAnnotations;

namespace Donation.Api.DTOs.Requests;

public class DonateRequest
{
    [Required(ErrorMessage = "Vui lòng chọn danh mục quyên góp")]
    public int CauseId { get; set; }

    [Required(ErrorMessage = "Họ và tên người quyên góp không được để trống")]
    [MaxLength(150, ErrorMessage = "Họ và tên không vượt quá 150 ký tự")]
    public string DonorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [MaxLength(256, ErrorMessage = "Email không vượt quá 256 ký tự")]
    public string DonorEmail { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [MaxLength(50, ErrorMessage = "Số điện thoại không vượt quá 50 ký tự")]
    public string? DonorPhone { get; set; }

    [Required(ErrorMessage = "Số tiền quyên góp không được để trống")]
    [Range(10000.00, 100000000.00, ErrorMessage = "Số tiền quyên góp phải từ 10.000 VNĐ đến 100.000.000 VNĐ")]
    public decimal Amount { get; set; }

    [MaxLength(500, ErrorMessage = "Lời nhắn không vượt quá 500 ký tự")]
    public string? Notes { get; set; }

    // Mock payment information
    [Required(ErrorMessage = "Tên chủ thẻ không được để trống")]
    [MaxLength(150, ErrorMessage = "Tên chủ thẻ không vượt quá 150 ký tự")]
    public string CardHolderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số thẻ không được để trống")]
    [RegularExpression(@"^[0-9]{16}$", ErrorMessage = "Số thẻ giả lập phải bao gồm 16 chữ số")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hạn thẻ không được để trống")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Hạn thẻ phải có định dạng MM/YY")]
    public string ExpiryDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã bảo mật CVV không được để trống")]
    [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "Mã CVV phải có 3 hoặc 4 chữ số")]
    public string Cvv { get; set; } = string.Empty;

    /// <summary>
    /// Kịch bản kiểm thử: Đặt là true nếu muốn ép buộc giao dịch giả lập thất bại
    /// </summary>
    public bool SimulateFailure { get; set; } = false;
}
