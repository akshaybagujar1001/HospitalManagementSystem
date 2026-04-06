using HMS.Application.DTOs.Fhir;
using HMS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services;

public class FhirExportService : IFhirExportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly DbContext _context;

    public FhirExportService(IUnitOfWork unitOfWork, DbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<FhirPatientDto> GetFhirPatientAsync(int patientId)
    {
        var patient = await _context.Set<HMS.Domain.Entities.Patient>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == patientId);
        
        if (patient == null) return null!;
        
        return new FhirPatientDto
        {
            ResourceType = "Patient",
            Id = patient.Id.ToString(),
            Identifier = new List<FhirIdentifierDto>
            {
                new FhirIdentifierDto { System = "http://hospital.com/patient-number", Value = patient.PatientNumber }
            },
            Name = new List<FhirHumanNameDto>
            {
                new FhirHumanNameDto
                {
                    Family = patient.User.LastName,
                    Given = new List<string> { patient.User.FirstName }
                }
            },
            Gender = patient.Gender.ToLower(),
            BirthDate = patient.DateOfBirth.ToString("yyyy-MM-dd"),
            Telecom = new List<FhirContactPointDto>
            {
                new FhirContactPointDto { System = "phone", Value = patient.User.PhoneNumber },
                new FhirContactPointDto { System = "email", Value = patient.User.Email }
            }
        };
    }

    public async Task<IEnumerable<FhirEncounterDto>> GetFhirEncountersAsync(int patientId)
    {
        var appointments = await _context.Set<HMS.Domain.Entities.Appointment>()
            .Include(a => a.Doctor)
            .ThenInclude(d => d.User)
            .Where(a => a.PatientId == patientId)
            .ToListAsync();
        
        return appointments.Select(a => new FhirEncounterDto
        {
            ResourceType = "Encounter",
            Id = a.Id.ToString(),
            Status = MapAppointmentStatus(a.Status),
            Class = new FhirCodingDto { Code = "AMB", Display = "ambulatory" },
            Subject = new FhirReferenceDto { Reference = $"Patient/{a.PatientId}" },
            Period = new FhirPeriodDto
            {
                Start = a.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                End = a.AppointmentDate.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ssZ")
            }
        });
    }

    public async Task<FhirObservationDto> GetFhirObservationAsync(int labTestId)
    {
        var labTest = await _context.Set<HMS.Domain.Entities.LabTest>()
            .Include(lt => lt.Patient)
            .Include(lt => lt.LabTestType)
            .FirstOrDefaultAsync(lt => lt.Id == labTestId);
        
        if (labTest == null) return null!;
        
        return new FhirObservationDto
        {
            ResourceType = "Observation",
            Id = labTest.Id.ToString(),
            Status = "final",
            Code = new FhirCodeableConceptDto
            {
                Coding = new List<FhirCodingDto>
                {
                    new FhirCodingDto { Code = labTest.LabTestType?.Name ?? "unknown", Display = labTest.LabTestType?.Name ?? "Unknown Test" }
                }
            },
            Subject = new FhirReferenceDto { Reference = $"Patient/{labTest.PatientId}" },
            EffectiveDateTime = (labTest.CompletedDate ?? labTest.RequestedDate).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            ValueString = labTest.Results ?? string.Empty
        };
    }

    private string MapAppointmentStatus(string status)
    {
        return status switch
        {
            "Scheduled" => "planned",
            "Completed" => "finished",
            "Cancelled" => "cancelled",
            _ => "unknown"
        };
    }
}

