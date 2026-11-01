namespace Identity.Api.DTOs.Responses;

public class RoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UserCount { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class PermissionResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class PermissionGroupResponse
{
    public string GroupName { get; set; } = string.Empty;
    public List<PermissionResponse> Permissions { get; set; } = new();
}
