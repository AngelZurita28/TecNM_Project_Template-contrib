using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TecNM.Api.Core.Entities;
using TecNM.Api.Data;
using TecNM.Api.Modules.People;

namespace TecNM.Api.Modules.Auth;

public sealed record JwtSettings(
    string Issuer,
    string Audience,
    int LifetimeHours,
    SymmetricSecurityKey SigningKey);

public sealed class AuthService(
    TecNMDbContext dbContext,
    PasswordHasher<User> passwordHasher,
    JwtSettings jwtSettings)
{
    public async Task<AuthSession?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();
        var user = await Accounts().SingleOrDefaultAsync(
            item => item.Username == normalizedUsername,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var verification = passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.Password = passwordHasher.HashPassword(user, password);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var expiresAt = DateTimeOffset.UtcNow.AddHours(jwtSettings.LifetimeHours);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Person.InstitutionalEmail),
            new Claim(JwtRegisteredClaimNames.Name, PersonMapping.FullName(user.Person)),
            new Claim("role", user.Role),
            new Claim("person_id", user.PersonId.ToString())
        };
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(
                jwtSettings.SigningKey,
                SecurityAlgorithms.HmacSha256));

        return new AuthSession(
            ToResponse(user),
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }

    public async Task<UserResponse?> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var id))
            return null;

        var user = await Accounts().AsNoTracking().SingleOrDefaultAsync(
            item => item.Id == id && item.IsActive, cancellationToken);
        return user is null ? null : ToResponse(user);
    }

    private IQueryable<User> Accounts() => dbContext.Users
        .Include(item => item.Person).ThenInclude(item => item.Student).ThenInclude(item => item!.Program)
        .Include(item => item.Person).ThenInclude(item => item.Employee).ThenInclude(item => item!.Department);

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.Username, user.Person.InstitutionalEmail, PersonMapping.FullName(user.Person), user.Role,
            user.IsActive, user.CreatedAt, PersonMapping.ToResponse(user.Person));
}
