using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Data;

public class HmsDbContext : DbContext
{
    public HmsDbContext(DbContextOptions<HmsDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<LabTest> LabTests { get; set; }
    public DbSet<LabTestType> LabTestTypes { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Bed> Beds { get; set; }
    public DbSet<Admission> Admissions { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    
    // New Enterprise Modules
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<InsuranceCompany> InsuranceCompanies { get; set; }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
    public DbSet<InsuranceClaim> InsuranceClaims { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<MedicationStockMovement> MedicationStockMovements { get; set; }
    public DbSet<PrescriptionFulfillment> PrescriptionFulfillments { get; set; }
    public DbSet<RadiologyOrder> RadiologyOrders { get; set; }
    public DbSet<RadiologyImage> RadiologyImages { get; set; }
    public DbSet<RadiologyReport> RadiologyReports { get; set; }
    public DbSet<NurseTask> NurseTasks { get; set; }
    public DbSet<EmergencyCase> EmergencyCases { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetMaintenance> AssetMaintenances { get; set; }
    public DbSet<AIDiagnosis> AIDiagnoses { get; set; }
    public DbSet<StaffShift> StaffShifts { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<RevenueRecord> RevenueRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired();
        });

        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasIndex(e => e.PatientNumber).IsUnique();
            entity.HasOne(e => e.User)
                .WithOne(e => e.Patient)
                .HasForeignKey<Patient>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Doctor configuration
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasIndex(e => e.DoctorNumber).IsUnique();
            entity.HasOne(e => e.User)
                .WithOne(e => e.Doctor)
                .HasForeignKey<Doctor>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.ConsultationFee).HasPrecision(18, 2);
        });

        // Nurse configuration
        modelBuilder.Entity<Nurse>(entity =>
        {
            entity.HasIndex(e => e.NurseNumber).IsUnique();
            entity.HasOne(e => e.User)
                .WithOne(e => e.Nurse)
                .HasForeignKey<Nurse>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Appointment configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointments");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AppointmentTime).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasOne(e => e.Patient)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Doctor)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasOne(e => e.Patient)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.FinalAmount).HasPrecision(18, 2);
        });

        // LabTest configuration
        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasIndex(e => e.TestNumber).IsUnique();
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
        
        // LabTestType configuration
        modelBuilder.Entity<LabTestType>(entity =>
        {
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
        
        // InvoiceItem configuration
        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
        });
        
        // Room configuration
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasIndex(e => e.RoomNumber).IsUnique();
            entity.Property(e => e.PricePerDay).HasPrecision(18, 2);
        });

        // Admission configuration
        modelBuilder.Entity<Admission>(entity =>
        {
            entity.HasIndex(e => e.AdmissionNumber).IsUnique();
        });

        // Room and Bed configuration
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasIndex(e => e.RoomNumber).IsUnique();
        });

        modelBuilder.Entity<Bed>(entity =>
        {
            entity.HasIndex(e => new { e.RoomId, e.BedNumber }).IsUnique();
        });
        
        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IPAddress).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Timestamp).IsRequired();
            
            // Foreign key to User (optional)
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Indexes for performance
            entity.HasIndex(e => new { e.EntityName, e.EntityId });
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);
        });
        
        // Insurance configuration
        modelBuilder.Entity<InsuranceCompany>(entity =>
        {
            entity.ToTable("InsuranceCompanies");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactInfo).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.PolicyRules).HasMaxLength(2000);
        });
        
        modelBuilder.Entity<InsurancePolicy>(entity =>
        {
            entity.HasIndex(e => e.PolicyNumber).IsUnique();
            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Company)
                .WithMany(e => e.InsurancePolicies)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<InsuranceClaim>(entity =>
        {
            entity.HasOne(e => e.Invoice)
                .WithMany()
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.AmountCovered).HasPrecision(18, 2);
            entity.Property(e => e.PatientResponsibility).HasPrecision(18, 2);
        });
        
        // Pharmacy configuration
        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
        
        // Radiology configuration
        modelBuilder.Entity<RadiologyOrder>(entity =>
        {
            entity.HasOne(e => e.Radiologist)
                .WithMany()
                .HasForeignKey(e => e.RadiologistId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.Cost).HasPrecision(18, 2);
        });
        
        modelBuilder.Entity<RadiologyReport>(entity =>
        {
            entity.HasOne(e => e.RadiologyOrder)
                .WithOne(e => e.Report)
                .HasForeignKey<RadiologyReport>(e => e.RadiologyOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Financial configuration
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });
        
        modelBuilder.Entity<RevenueRecord>(entity =>
        {
            entity.ToTable("RevenueRecords");
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(e => e.Invoice)
                .WithMany()
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}

