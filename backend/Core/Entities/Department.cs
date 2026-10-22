namespace TecNM.Api.Core.Entities;

public sealed class Department
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
}
