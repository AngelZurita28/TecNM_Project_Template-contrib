namespace TecNM.Api.Core.Entities;

public sealed class Person
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? SecondLastName { get; set; }
    public required string InstitutionalEmail { get; set; }
    public string? PersonalEmail { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? BirthPlace { get; set; }
    public string? Sex { get; set; }
    public string? Nationality { get; set; }
    public string? Curp { get; set; }
    public string? Rfc { get; set; }
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Street { get; set; }
    public string? AddressNumber { get; set; }
    public string? Neighborhood { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    public Student? Student { get; set; }
    public Employee? Employee { get; set; }
}
