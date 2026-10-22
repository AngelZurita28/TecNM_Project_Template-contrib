namespace TecNM.Api.Core.Entities;

public sealed class User
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public Person Person { get; set; } = null!;
}
