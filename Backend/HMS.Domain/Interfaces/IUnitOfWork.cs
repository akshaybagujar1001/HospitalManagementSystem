using HMS.Domain.Entities;

namespace HMS.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Patient> Patients { get; }
    IRepository<Doctor> Doctors { get; }
    IRepository<Nurse> Nurses { get; }
    IRepository<Appointment> Appointments { get; }
    IRepository<MedicalRecord> MedicalRecords { get; }
    IRepository<Prescription> Prescriptions { get; }
    IRepository<LabTest> LabTests { get; }
    IRepository<LabTestType> LabTestTypes { get; }
    IRepository<Invoice> Invoices { get; }
    IRepository<InvoiceItem> InvoiceItems { get; }
    IRepository<Room> Rooms { get; }
    IRepository<Bed> Beds { get; }
    IRepository<Admission> Admissions { get; }
    IRepository<DoctorSchedule> DoctorSchedules { get; }
    
    // New Enterprise Modules
    IRepository<Notification> Notifications { get; }
    IRepository<AuditLog> AuditLogs { get; }
    IRepository<InsuranceCompany> InsuranceCompanies { get; }
    IRepository<InsurancePolicy> InsurancePolicies { get; }
    IRepository<InsuranceClaim> InsuranceClaims { get; }
    IRepository<Medication> Medications { get; }
    IRepository<MedicationStockMovement> MedicationStockMovements { get; }
    IRepository<PrescriptionFulfillment> PrescriptionFulfillments { get; }
    IRepository<RadiologyOrder> RadiologyOrders { get; }
    IRepository<RadiologyImage> RadiologyImages { get; }
    IRepository<RadiologyReport> RadiologyReports { get; }
    IRepository<NurseTask> NurseTasks { get; }
    IRepository<EmergencyCase> EmergencyCases { get; }
    IRepository<Asset> Assets { get; }
    IRepository<AssetMaintenance> AssetMaintenances { get; }
    IRepository<AIDiagnosis> AIDiagnoses { get; }
    IRepository<StaffShift> StaffShifts { get; }
    IRepository<Expense> Expenses { get; }
    IRepository<RevenueRecord> RevenueRecords { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

