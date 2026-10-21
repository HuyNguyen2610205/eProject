namespace Identity.Api.Data.Entities;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public virtual ApplicationRole Role { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public virtual Permission Permission { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
