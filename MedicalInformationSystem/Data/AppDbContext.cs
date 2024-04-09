using MedicalInformationSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalInformationSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<CommentEntity> Comment { get; set; }
    public DbSet<ConsultationEntity> Consultation { get; set; }
    public DbSet<DiagnosisEntity> Diagnosis { get; set; }
    public DbSet<DoctorEntity> Doctor { get; set; }
    public DbSet<InspectionEntity> Inspection { get; set; }
    public DbSet<PatientEntity> Patient { get; set; }
    public DbSet<SpecialityEntity> Speciality { get; set; }
    public DbSet<PasswordEntity> Password { get; set; }
    
    public DbSet<IcdRootsEntity> IcdRoots { get; set; }
    public DbSet<IcdEntity> Icd { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommentEntity>(options =>
        {
            options.ToTable("Comment");
        });
        
        modelBuilder.Entity<ConsultationEntity>(options =>
        {
            options.ToTable("Consultation");
        });
        
        modelBuilder.Entity<DiagnosisEntity>(options =>
        {
            options.ToTable("Diagnosis");
        });
        
        modelBuilder.Entity<DoctorEntity>(options =>
        {
            options.ToTable("Doctor");
            options.HasIndex(x => x.Email).IsUnique();
        });
        
        modelBuilder.Entity<InspectionEntity>(options =>
        {
            /*options.HasOne(x => x.PreviousInspection)
                .WithOne(x => x.NextInspection);
            options.HasOne(x => x.NextInspection)
                .WithOne(x => x.PreviousInspection);*/
            options.ToTable("Inspection");
        });
        
        modelBuilder.Entity<PatientEntity>(options =>
        {
            options.ToTable("Patient");
            options.HasIndex(x => x.Name);
        });
        
        modelBuilder.Entity<SpecialityEntity>(options =>
        {
            options.ToTable("Speciality");
            options.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<PasswordEntity>(options =>
        {
            options.ToTable("Password");
        });

        modelBuilder.Entity<IcdRootsEntity>(options =>
        {
            options.HasIndex(x => x.NewId);
            options.HasIndex(x => x.MKB_CODE);
            options.HasIndex(x => x.MKB_NAME);
        });
        
        modelBuilder.Entity<IcdEntity>(options =>
        {
            options.HasIndex(x => x.NewId);
            options.HasIndex(x => x.MKB_CODE);
            options.HasIndex(x => x.MKB_NAME);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}