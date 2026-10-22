namespace TecNM.Api.Core.Entities;

public sealed class Employee
{
    public Guid PersonId { get; set; }
    public required string EmployeeNumber { get; set; }
    public Guid DepartmentId { get; set; }
    public required string JobTitle { get; set; }
    public DateOnly? HireDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTeacher { get; set; }
    public bool IsAdministrative { get; set; }
    public string? Degree { get; set; }
    public string? ProfessionalLicense { get; set; }
    public Person Person { get; set; } = null!;
    public Department Department { get; set; } = null!;
}
