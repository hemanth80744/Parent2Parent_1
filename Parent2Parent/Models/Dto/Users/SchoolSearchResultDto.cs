namespace Parent2Parent.Models.Dto.Users;

public sealed class SchoolSearchResultDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ChildClass { get; set; }
}

