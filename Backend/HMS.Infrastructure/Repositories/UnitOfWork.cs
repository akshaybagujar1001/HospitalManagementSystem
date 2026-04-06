using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace HMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HmsDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<User>? _users;
    private IRepository<Patient>? _patients;
    private IRepository<Doctor>? _doctors;
    private IRepository<Nurse>? _nurses;
    private IRepository<Appointment>? _appointments;
    private IRepository<MedicalRecord>? _medicalRecords;
    private IRepository<Prescription>? _prescriptions;
    private IRepository<LabTest>? _labTests;
    private IRepository<LabTestType>? _labTestTypes;
    private IRepository<Invoice>? _invoices;
    private IRepository<InvoiceItem>? _invoiceItems;
    private IRepository<Room>? _rooms;
    private IRepository<Bed>? _beds;
    private IRepository<Admission>? _admissions;
    private IRepository<DoctorSchedule>? _doctorSchedules;
    
    // New Enterprise Modules
    private IRepository<Notification>? _notifications;
    private IRepository<AuditLog>? _auditLogs;
    private IRepository<InsuranceCompany>? _insuranceCompanies;
    private IRepository<InsurancePolicy>? _insurancePolicies;
    private IRepository<InsuranceClaim>? _insuranceClaims;
    private IRepository<Medication>? _medications;
    private IRepository<MedicationStockMovement>? _medicationStockMovements;
    private IRepository<PrescriptionFulfillment>? _prescriptionFulfillments;
    private IRepository<RadiologyOrder>? _radiologyOrders;
    private IRepository<RadiologyImage>? _radiologyImages;
    private IRepository<RadiologyReport>? _radiologyReports;
    private IRepository<NurseTask>? _nurseTasks;
    private IRepository<EmergencyCase>? _emergencyCases;
    private IRepository<Asset>? _assets;
    private IRepository<AssetMaintenance>? _assetMaintenances;
    private IRepository<AIDiagnosis>? _aiDiagnoses;
    private IRepository<StaffShift>? _staffShifts;
    private IRepository<Expense>? _expenses;
    private IRepository<RevenueRecord>? _revenueRecords;

    public UnitOfWork(HmsDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Patient> Patients => _patients ??= new Repository<Patient>(_context);
    public IRepository<Doctor> Doctors => _doctors ??= new Repository<Doctor>(_context);
    public IRepository<Nurse> Nurses => _nurses ??= new Repository<Nurse>(_context);
    public IRepository<Appointment> Appointments => _appointments ??= new Repository<Appointment>(_context);
    public IRepository<MedicalRecord> MedicalRecords => _medicalRecords ??= new Repository<MedicalRecord>(_context);
    public IRepository<Prescription> Prescriptions => _prescriptions ??= new Repository<Prescription>(_context);
    public IRepository<LabTest> LabTests => _labTests ??= new Repository<LabTest>(_context);
    public IRepository<LabTestType> LabTestTypes => _labTestTypes ??= new Repository<LabTestType>(_context);
    public IRepository<Invoice> Invoices => _invoices ??= new Repository<Invoice>(_context);
    public IRepository<InvoiceItem> InvoiceItems => _invoiceItems ??= new Repository<InvoiceItem>(_context);
    public IRepository<Room> Rooms => _rooms ??= new Repository<Room>(_context);
    public IRepository<Bed> Beds => _beds ??= new Repository<Bed>(_context);
    public IRepository<Admission> Admissions => _admissions ??= new Repository<Admission>(_context);
    public IRepository<DoctorSchedule> DoctorSchedules => _doctorSchedules ??= new Repository<DoctorSchedule>(_context);
    
    // New Enterprise Modules
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);
    public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_context);
    public IRepository<InsuranceCompany> InsuranceCompanies => _insuranceCompanies ??= new Repository<InsuranceCompany>(_context);
    public IRepository<InsurancePolicy> InsurancePolicies => _insurancePolicies ??= new Repository<InsurancePolicy>(_context);
    public IRepository<InsuranceClaim> InsuranceClaims => _insuranceClaims ??= new Repository<InsuranceClaim>(_context);
    public IRepository<Medication> Medications => _medications ??= new Repository<Medication>(_context);
    public IRepository<MedicationStockMovement> MedicationStockMovements => _medicationStockMovements ??= new Repository<MedicationStockMovement>(_context);
    public IRepository<PrescriptionFulfillment> PrescriptionFulfillments => _prescriptionFulfillments ??= new Repository<PrescriptionFulfillment>(_context);
    public IRepository<RadiologyOrder> RadiologyOrders => _radiologyOrders ??= new Repository<RadiologyOrder>(_context);
    public IRepository<RadiologyImage> RadiologyImages => _radiologyImages ??= new Repository<RadiologyImage>(_context);
    public IRepository<RadiologyReport> RadiologyReports => _radiologyReports ??= new Repository<RadiologyReport>(_context);
    public IRepository<NurseTask> NurseTasks => _nurseTasks ??= new Repository<NurseTask>(_context);
    public IRepository<EmergencyCase> EmergencyCases => _emergencyCases ??= new Repository<EmergencyCase>(_context);
    public IRepository<Asset> Assets => _assets ??= new Repository<Asset>(_context);
    public IRepository<AssetMaintenance> AssetMaintenances => _assetMaintenances ??= new Repository<AssetMaintenance>(_context);
    public IRepository<AIDiagnosis> AIDiagnoses => _aiDiagnoses ??= new Repository<AIDiagnosis>(_context);
    public IRepository<StaffShift> StaffShifts => _staffShifts ??= new Repository<StaffShift>(_context);
    public IRepository<Expense> Expenses => _expenses ??= new Repository<Expense>(_context);
    public IRepository<RevenueRecord> RevenueRecords => _revenueRecords ??= new Repository<RevenueRecord>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

