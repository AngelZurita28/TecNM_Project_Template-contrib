namespace TecNM.Api.Modules.People;

public sealed record CatalogResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive);

public sealed record StudentResponse(
    Guid PersonId,
    string StudentNumber,
    Guid ProgramId,
    int AdmissionYear,
    string? AdmissionTerm,
    string? Generation,
    string? Modality,
    string? CurriculumCode,
    string? Specialty,
    string Status,
    bool IsEnrolled,
    CatalogResponse Program);

public sealed record EmployeeResponse(
    Guid PersonId,
    string EmployeeNumber,
    Guid DepartmentId,
    string JobTitle,
    DateOnly? HireDate,
    bool IsActive,
    bool IsTeacher,
    bool IsAdministrative,
    string? Degree,
    string? ProfessionalLicense,
    CatalogResponse Department);

public sealed record PersonResponse(
    Guid Id,
    string FirstName,
    string? LastName,
    string? SecondLastName,
    string InstitutionalEmail,
    string? PersonalEmail,
    DateOnly? BirthDate,
    string? BirthPlace,
    string? Sex,
    string? Nationality,
    string? Curp,
    string? Rfc,
    string? Phone,
    string? MobilePhone,
    string? Street,
    string? AddressNumber,
    string? Neighborhood,
    string? PostalCode,
    string? City,
    string? State,
    string? Country,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? EmergencyContactRelationship,
    StudentResponse? Student,
    EmployeeResponse? Employee);

public sealed record DirectoryPersonResponse(
    Guid PersonId, string Name, string InstitutionalEmail,
    StudentResponse? Student, DirectoryEmployeeResponse? Employee);

public sealed record DirectoryEmployeeResponse(
    Guid PersonId, string EmployeeNumber, string JobTitle, DateOnly? HireDate,
    bool IsActive, bool IsTeacher, bool IsAdministrative, CatalogResponse Department);

internal static class PersonMapping
{
    internal static string FullName(TecNM.Api.Core.Entities.Person person) =>
        string.Join(" ", new[] { person.FirstName, person.LastName, person.SecondLastName }.Where(value => !string.IsNullOrWhiteSpace(value)));

    internal static StudentResponse ToResponse(TecNM.Api.Core.Entities.Student item) =>
        new(item.PersonId,
            item.StudentNumber,
            item.ProgramId,
            item.AdmissionYear,
            item.AdmissionTerm,
            item.Generation,
            item.Modality,
            item.CurriculumCode,
            item.Specialty,
            item.Status,
            item.IsEnrolled,
            new CatalogResponse(item.Program.Id, item.Program.Code, item.Program.Name, item.Program.IsActive));

    internal static EmployeeResponse ToResponse(TecNM.Api.Core.Entities.Employee item) =>
        new(item.PersonId,
            item.EmployeeNumber,
            item.DepartmentId,
            item.JobTitle,
            item.HireDate,
            item.IsActive,
            item.IsTeacher,
            item.IsAdministrative,
            item.Degree,
            item.ProfessionalLicense,
            new CatalogResponse(item.Department.Id, item.Department.Code, item.Department.Name, item.Department.IsActive));

    internal static PersonResponse ToResponse(TecNM.Api.Core.Entities.Person item) =>
        new(item.Id,
            item.FirstName,
            item.LastName,
            item.SecondLastName,
            item.InstitutionalEmail,
            item.PersonalEmail,
            item.BirthDate,
            item.BirthPlace,
            item.Sex,
            item.Nationality,
            item.Curp,
            item.Rfc,
            item.Phone,
            item.MobilePhone,
            item.Street,
            item.AddressNumber,
            item.Neighborhood,
            item.PostalCode,
            item.City,
            item.State,
            item.Country,
            item.EmergencyContactName,
            item.EmergencyContactPhone,
            item.EmergencyContactRelationship,
            item.Student is null ? null : ToResponse(item.Student),
            item.Employee is null ? null : ToResponse(item.Employee));

}
