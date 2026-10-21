namespace Identity.Api.Data.Entities;

public class UserPermission
{
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public virtual Permission Permission { get; set; } = null!;

    public bool IsGranted { get; set; } = true; // true = Cho phép thêm, false = Chặn quyền (Deny)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
