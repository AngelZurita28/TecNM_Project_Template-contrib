using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TecNM.Api.Modules.People;

[Authorize]
[ApiController]
[Route("api/people")]
public sealed class PeopleController(PeopleService peopleService) : ControllerBase
{
    [HttpGet("{personId:guid}")]
    public async Task<ActionResult<DirectoryPersonResponse>> GetById(Guid personId, CancellationToken cancellationToken)
    {
        var person = await peopleService.FindAsync(personId, null, null, null, cancellationToken);
        return person is null ? NotFound() : Ok(person);
    }

    [HttpGet]
    public async Task<ActionResult<DirectoryPersonResponse>> Find(
        [FromQuery] string? studentNumber, [FromQuery] string? employeeNumber,
        [FromQuery] string? username, CancellationToken cancellationToken)
    {
        var filters = new[] { studentNumber, employeeNumber, username };
        if (filters.Count(value => value is not null) != 1
            || filters.Any(value => value is not null && (string.IsNullOrWhiteSpace(value) || value.Length > 64))
            || Request.Query.Any(item => item.Value.Count != 1
                || item.Key is not ("studentNumber" or "employeeNumber" or "username")))
            return BadRequest(new { message = "Indica únicamente una matrícula, número de empleado o usuario válido." });

        var person = await peopleService.FindAsync(null, studentNumber?.Trim().ToUpperInvariant(),
            employeeNumber?.Trim().ToUpperInvariant(), username?.Trim().ToLowerInvariant(), cancellationToken);
        return person is null ? NotFound() : Ok(person);
    }
}
