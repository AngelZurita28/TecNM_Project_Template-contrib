namespace TecNM.Api.Core.Entities;

public sealed class Student
{
    public Guid PersonId { get; set; }
    public required string StudentNumber { get; set; }
    public Guid ProgramId { get; set; }
    public int AdmissionYear { get; set; }
    public string? AdmissionTerm { get; set; }
    public string? Generation { get; set; }
    public string? Modality { get; set; }
    public string? CurriculumCode { get; set; }
    public string? Specialty { get; set; }
    public required string Status { get; set; }
    public bool IsEnrolled { get; set; }
    public Person Person { get; set; } = null!;
    public AcademicProgram Program { get; set; } = null!;
}
