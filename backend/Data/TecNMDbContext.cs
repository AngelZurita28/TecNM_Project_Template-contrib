using Microsoft.EntityFrameworkCore;
using TecNM.Api.Core.Entities;

namespace TecNM.Api.Data;

public sealed class TecNMDbContext(DbContextOptions<TecNMDbContext> options) : DbContext(options)
{
    public DbSet<Person> People => Set<Person>();
    public DbSet<AcademicProgram> Programs => Set<AcademicProgram>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var people = modelBuilder.Entity<Person>();
        people.ToTable("people", table =>
        {
            table.HasCheckConstraint("ck_people_email", "institutional_email = lower(btrim(institutional_email)) AND institutional_email ~ '^[^[:space:]@]+@[^[:space:]@]+[.][^[:space:]@]+$'");
            table.HasCheckConstraint("ck_people_name", "length(btrim(first_name)) > 0");
            table.HasCheckConstraint("ck_people_curp", "curp IS NULL OR curp ~ '^[A-Z0-9]{18}$'");
            table.HasCheckConstraint("ck_people_rfc", "rfc IS NULL OR rfc ~ '^[A-ZÑ&]{3,4}[0-9]{6}[A-Z0-9]{3}$'");
        });
        people.HasKey(item => item.Id).HasName("pk_people");
        people.Property(item => item.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").IsRequired();
        people.Property(item => item.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        people.Property(item => item.LastName).HasColumnName("last_name").HasMaxLength(100);
        people.Property(item => item.SecondLastName).HasColumnName("second_last_name").HasMaxLength(100);
        people.Property(item => item.InstitutionalEmail).HasColumnName("institutional_email").HasMaxLength(254).IsRequired();
        people.Property(item => item.PersonalEmail).HasColumnName("personal_email").HasMaxLength(254);
        people.Property(item => item.BirthDate).HasColumnName("birth_date");
        people.Property(item => item.BirthPlace).HasColumnName("birth_place").HasMaxLength(160);
        people.Property(item => item.Sex).HasColumnName("sex").HasMaxLength(30);
        people.Property(item => item.Nationality).HasColumnName("nationality").HasMaxLength(80);
        people.Property(item => item.Curp).HasColumnName("curp").HasMaxLength(18);
        people.Property(item => item.Rfc).HasColumnName("rfc").HasMaxLength(13);
        people.Property(item => item.Phone).HasColumnName("phone").HasMaxLength(30);
        people.Property(item => item.MobilePhone).HasColumnName("mobile_phone").HasMaxLength(30);
        people.Property(item => item.Street).HasColumnName("street").HasMaxLength(120);
        people.Property(item => item.AddressNumber).HasColumnName("address_number").HasMaxLength(30);
        people.Property(item => item.Neighborhood).HasColumnName("neighborhood").HasMaxLength(100);
        people.Property(item => item.PostalCode).HasColumnName("postal_code").HasMaxLength(10);
        people.Property(item => item.City).HasColumnName("city").HasMaxLength(100);
        people.Property(item => item.State).HasColumnName("state").HasMaxLength(100);
        people.Property(item => item.Country).HasColumnName("country").HasMaxLength(80);
        people.Property(item => item.EmergencyContactName).HasColumnName("emergency_contact_name").HasMaxLength(160);
        people.Property(item => item.EmergencyContactPhone).HasColumnName("emergency_contact_phone").HasMaxLength(30);
        people.Property(item => item.EmergencyContactRelationship).HasColumnName("emergency_contact_relationship").HasMaxLength(60);
        people.HasIndex(item => item.InstitutionalEmail).IsUnique().HasDatabaseName("uq_people_institutional_email");
        people.HasIndex(item => item.Curp).IsUnique().HasDatabaseName("uq_people_curp");
        people.HasIndex(item => item.Rfc).IsUnique().HasDatabaseName("uq_people_rfc");

        var programs = modelBuilder.Entity<AcademicProgram>();
        programs.ToTable("programs", table =>
        {
            table.HasCheckConstraint("ck_programs_code", "length(btrim(code)) > 0");
        });
        programs.HasKey(item => item.Id).HasName("pk_programs");
        programs.Property(item => item.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").IsRequired();
        programs.Property(item => item.Code).HasColumnName("code").HasMaxLength(30).IsRequired();
        programs.Property(item => item.Name).HasColumnName("name").HasMaxLength(160).IsRequired();
        programs.Property(item => item.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        programs.HasIndex(item => item.Code).IsUnique().HasDatabaseName("uq_programs_code");

        var departments = modelBuilder.Entity<Department>();
        departments.ToTable("departments", table =>
        {
            table.HasCheckConstraint("ck_departments_code", "length(btrim(code)) > 0");
        });
        departments.HasKey(item => item.Id).HasName("pk_departments");
        departments.Property(item => item.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").IsRequired();
        departments.Property(item => item.Code).HasColumnName("code").HasMaxLength(30).IsRequired();
        departments.Property(item => item.Name).HasColumnName("name").HasMaxLength(160).IsRequired();
        departments.Property(item => item.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        departments.HasIndex(item => item.Code).IsUnique().HasDatabaseName("uq_departments_code");

        var students = modelBuilder.Entity<Student>();
        students.ToTable("students", table =>
        {
            table.HasCheckConstraint("ck_students_student_number", "student_number = upper(btrim(student_number)) AND length(student_number) > 0");
            table.HasCheckConstraint("ck_students_admission_year", "admission_year BETWEEN 1900 AND 2200");
            table.HasCheckConstraint("ck_students_status", "status IN ('Active', 'TemporaryLeave', 'Withdrawn', 'Graduated')");
            table.HasCheckConstraint("ck_students_enrollment", "NOT is_enrolled OR status = 'Active'");
        });
        students.HasKey(item => item.PersonId).HasName("pk_students");
        students.Property(item => item.PersonId).HasColumnName("person_id").IsRequired();
        students.Property(item => item.StudentNumber).HasColumnName("student_number").HasMaxLength(20).IsRequired();
        students.Property(item => item.ProgramId).HasColumnName("program_id").IsRequired();
        students.Property(item => item.AdmissionYear).HasColumnName("admission_year").IsRequired();
        students.Property(item => item.AdmissionTerm).HasColumnName("admission_term").HasMaxLength(40);
        students.Property(item => item.Generation).HasColumnName("generation").HasMaxLength(30);
        students.Property(item => item.Modality).HasColumnName("modality").HasMaxLength(40);
        students.Property(item => item.CurriculumCode).HasColumnName("curriculum_code").HasMaxLength(50);
        students.Property(item => item.Specialty).HasColumnName("specialty").HasMaxLength(160);
        students.Property(item => item.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        students.Property(item => item.IsEnrolled).HasColumnName("is_enrolled").IsRequired();
        students.HasIndex(item => item.StudentNumber).IsUnique().HasDatabaseName("uq_students_student_number");
        students.HasOne(item => item.Person).WithOne(item => item.Student).HasForeignKey<Student>(item => item.PersonId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_students_people");
        students.HasOne(item => item.Program).WithMany().HasForeignKey(item => item.ProgramId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_students_programs");
        students.HasIndex(item => item.ProgramId).HasDatabaseName("ix_students_program_id");

        var employees = modelBuilder.Entity<Employee>();
        employees.ToTable("employees", table =>
        {
            table.HasCheckConstraint("ck_employees_employee_number", "employee_number = upper(btrim(employee_number)) AND length(employee_number) > 0");
            table.HasCheckConstraint("ck_employees_type", "is_teacher OR is_administrative");
        });
        employees.HasKey(item => item.PersonId).HasName("pk_employees");
        employees.Property(item => item.PersonId).HasColumnName("person_id").IsRequired();
        employees.Property(item => item.EmployeeNumber).HasColumnName("employee_number").HasMaxLength(20).IsRequired();
        employees.Property(item => item.DepartmentId).HasColumnName("department_id").IsRequired();
        employees.Property(item => item.JobTitle).HasColumnName("job_title").HasMaxLength(160).IsRequired();
        employees.Property(item => item.HireDate).HasColumnName("hire_date");
        employees.Property(item => item.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        employees.Property(item => item.IsTeacher).HasColumnName("is_teacher").IsRequired();
        employees.Property(item => item.IsAdministrative).HasColumnName("is_administrative").IsRequired();
        employees.Property(item => item.Degree).HasColumnName("degree").HasMaxLength(160);
        employees.Property(item => item.ProfessionalLicense).HasColumnName("professional_license").HasMaxLength(30);
        employees.HasIndex(item => item.EmployeeNumber).IsUnique().HasDatabaseName("uq_employees_employee_number");
        employees.HasOne(item => item.Person).WithOne(item => item.Employee).HasForeignKey<Employee>(item => item.PersonId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_employees_people");
        employees.HasOne(item => item.Department).WithMany().HasForeignKey(item => item.DepartmentId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_employees_departments");
        employees.HasIndex(item => item.DepartmentId).HasDatabaseName("ix_employees_department_id");

        var users = modelBuilder.Entity<User>();
        users.ToTable("users", table =>
        {
            table.HasCheckConstraint("ck_users_role", "role IN ('Admin', 'User')");
            table.HasCheckConstraint("ck_users_username", "username = lower(btrim(username)) AND length(username) > 0");
        });
        users.HasKey(item => item.Id).HasName("pk_users");
        users.Property(item => item.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()").IsRequired();
        users.Property(item => item.PersonId).HasColumnName("person_id").IsRequired();
        users.Property(item => item.Username).HasColumnName("username").HasMaxLength(64).IsRequired();
        users.Property(item => item.Password).HasColumnName("password").HasMaxLength(512).IsRequired();
        users.Property(item => item.Role).HasColumnName("role").HasMaxLength(16).IsRequired();
        users.Property(item => item.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
        users.Property(item => item.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
        users.HasIndex(item => item.Username).IsUnique().HasDatabaseName("uq_users_username");
        users.HasIndex(item => item.PersonId).IsUnique().HasDatabaseName("uq_users_person_id");
        users.HasOne(item => item.Person).WithOne().HasForeignKey<User>(item => item.PersonId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_users_people");

    }
}
