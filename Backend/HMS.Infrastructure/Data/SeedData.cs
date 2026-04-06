using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Data;

public static class SeedData
{
    public static void Seed(HmsDbContext context)
    {
        context.Database.EnsureCreated();

        var adminUser = EnsureUser(
            context,
            email: "admin@hms.com",
            firstName: "Admin",
            lastName: "User",
            role: "Admin",
            phoneNumber: "1234567890",
            password: "Admin@123");

        var doctorUser = EnsureUser(
            context,
            email: "doctor@hms.com",
            firstName: "John",
            lastName: "Doctor",
            role: "Doctor",
            phoneNumber: "1234567891",
            password: "Doctor@123");

        var nurseUser = EnsureUser(
            context,
            email: "nurse@hms.com",
            firstName: "Jane",
            lastName: "Nurse",
            role: "Nurse",
            phoneNumber: "1234567892",
            password: "Nurse@123");

        var patientUser = EnsureUser(
            context,
            email: "patient@hms.com",
            firstName: "Patient",
            lastName: "User",
            role: "Patient",
            phoneNumber: "1234567893",
            password: "Patient@123");

        context.SaveChanges();

        EnsureDoctor(context, doctorUser);
        EnsureNurse(context, nurseUser);
        EnsurePatient(context, patientUser);

        if (!context.LabTestTypes.Any())
        {
            var labTestTypes = new[]
            {
                new LabTestType { Name = "Blood Test", Description = "Complete Blood Count", Price = 500, Category = "Blood Test", IsActive = true, CreatedAt = DateTime.UtcNow },
                new LabTestType { Name = "X-Ray", Description = "Chest X-Ray", Price = 800, Category = "Imaging", IsActive = true, CreatedAt = DateTime.UtcNow },
                new LabTestType { Name = "Urine Test", Description = "Urine Analysis", Price = 300, Category = "Urine Test", IsActive = true, CreatedAt = DateTime.UtcNow },
                new LabTestType { Name = "ECG", Description = "Electrocardiogram", Price = 600, Category = "Cardiac", IsActive = true, CreatedAt = DateTime.UtcNow }
            };
            context.LabTestTypes.AddRange(labTestTypes);
        }

        if (!context.Rooms.Any())
        {
            var room1 = new Room
            {
                RoomNumber = "R001",
                RoomType = "General",
                Floor = "1st Floor",
                Description = "General ward room",
                PricePerDay = 2000,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Rooms.Add(room1);

            var room2 = new Room
            {
                RoomNumber = "R002",
                RoomType = "Private",
                Floor = "2nd Floor",
                Description = "Private room with AC",
                PricePerDay = 5000,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Rooms.Add(room2);

            context.SaveChanges();

            var beds = new[]
            {
                new Bed { RoomId = room1.Id, BedNumber = "B001", IsOccupied = false, IsAvailable = true, CreatedAt = DateTime.UtcNow },
                new Bed { RoomId = room1.Id, BedNumber = "B002", IsOccupied = false, IsAvailable = true, CreatedAt = DateTime.UtcNow },
                new Bed { RoomId = room2.Id, BedNumber = "B001", IsOccupied = false, IsAvailable = true, CreatedAt = DateTime.UtcNow }
            };
            context.Beds.AddRange(beds);
            context.SaveChanges();
        }
    }

    private static User EnsureUser(
        HmsDbContext context,
        string email,
        string firstName,
        string lastName,
        string role,
        string phoneNumber,
        string password)
    {
        var user = context.Users.FirstOrDefault(u => u.Email == email);
        var passwordHash = HashPassword(password);

        if (user == null)
        {
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                Role = role,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
        }
        else
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;
            user.Role = role;
            user.PasswordHash = passwordHash;
            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            context.Users.Update(user);
        }

        return user;
    }

    private static void EnsureDoctor(HmsDbContext context, User doctorUser)
    {
        if (context.Doctors.Any(d => d.UserId == doctorUser.Id))
            return;

        context.Doctors.Add(new Doctor
        {
            UserId = doctorUser.Id,
            DoctorNumber = "DOC-20240101-001",
            Specialization = "Cardiology",
            LicenseNumber = "LIC-12345",
            PhoneNumber = doctorUser.PhoneNumber,
            Bio = "Experienced cardiologist with 10+ years of experience",
            ConsultationFee = 500,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        });
    }

    private static void EnsureNurse(HmsDbContext context, User nurseUser)
    {
        if (context.Nurses.Any(n => n.UserId == nurseUser.Id))
            return;

        context.Nurses.Add(new Nurse
        {
            UserId = nurseUser.Id,
            NurseNumber = "NUR-20240101-001",
            LicenseNumber = "NUR-12345",
            PhoneNumber = nurseUser.PhoneNumber,
            Department = "Emergency",
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        });
    }

    private static void EnsurePatient(HmsDbContext context, User patientUser)
    {
        if (context.Patients.Any(p => p.UserId == patientUser.Id))
            return;

        context.Patients.Add(new Patient
        {
            UserId = patientUser.Id,
            PatientNumber = "PAT-20240101-001",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            Address = "123 Main St, City",
            BloodGroup = "O+",
            Allergies = "None",
            EmergencyContactName = "Emergency Contact",
            EmergencyContactPhone = "9876543210",
            CreatedAt = DateTime.UtcNow
        });

        context.SaveChanges();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

