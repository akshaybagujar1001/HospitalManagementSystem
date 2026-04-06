using AutoMapper;
using HMS.Application.DTOs.Appointment;
using HMS.Application.DTOs.Auth;
using HMS.Application.DTOs.Doctor;
using HMS.Application.DTOs.Invoice;
using HMS.Application.DTOs.Patient;
using HMS.Domain.Entities;

namespace HMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth mappings
        CreateMap<RegisterDto, User>();
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
        
        // Patient mappings
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));
        
        // Doctor mappings
        CreateMap<Doctor, DoctorDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));
        
        // Appointment mappings
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.User.FirstName} {src.Patient.User.LastName}"))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => $"{src.Doctor.User.FirstName} {src.Doctor.User.LastName}"))
            .ForMember(dest => dest.DoctorSpecialization, opt => opt.MapFrom(src => src.Doctor.Specialization));
        
        // Invoice mappings
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.User.FirstName} {src.Patient.User.LastName}"));
        
        CreateMap<InvoiceItem, InvoiceItemDto>();
    }
}

