using TecNM.Api.Modules.People;

namespace TecNM.Api.Modules.Auth;

public sealed record LoginRequest(string? Username, string? Password);

public sealed record UserResponse(
    Guid Id, string Username, string Email, string Name, string Role,
    bool IsActive, DateTimeOffset CreatedAt, PersonResponse Person);

public sealed record AuthSession(UserResponse User, string Token, DateTimeOffset ExpiresAt);
