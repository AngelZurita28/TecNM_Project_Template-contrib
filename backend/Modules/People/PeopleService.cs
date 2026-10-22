using Microsoft.EntityFrameworkCore;
using TecNM.Api.Data;

namespace TecNM.Api.Modules.People;

public sealed class PeopleService(TecNMDbContext dbContext)
{
    public async Task<DirectoryPersonResponse?> FindAsync(
        Guid? personId, string? studentNumber, string? employeeNumber, string? username,
        CancellationToken cancellationToken)
    {
        var query = dbContext.People.AsNoTracking();
        if (personId.HasValue)
            query = query.Where(item => item.Id == personId.Value);
        else if (studentNumber is not null)
            query = query.Where(item => item.Student != null && item.Student.StudentNumber == studentNumber);
        else if (employeeNumber is not null)
            query = query.Where(item => item.Employee != null && item.Employee.EmployeeNumber == employeeNumber);
        else
            query = query.Where(item => dbContext.Users.Any(account => account.PersonId == item.Id && account.Username == username));

        var person = await query
            .Include(item => item.Student).ThenInclude(item => item!.Program)
            .Include(item => item.Employee).ThenInclude(item => item!.Department)
            .SingleOrDefaultAsync(cancellationToken);
        if (person is null) return null;

        var employee = person.Employee;
        return new DirectoryPersonResponse(person.Id, PersonMapping.FullName(person), person.InstitutionalEmail,
            person.Student is null ? null : PersonMapping.ToResponse(person.Student),
            employee is null ? null : new DirectoryEmployeeResponse(
                employee.PersonId, employee.EmployeeNumber, employee.JobTitle, employee.HireDate,
                employee.IsActive, employee.IsTeacher, employee.IsAdministrative,
                new CatalogResponse(employee.Department.Id, employee.Department.Code,
                    employee.Department.Name, employee.Department.IsActive)));
    }
}
