# Database Schema Documentation

## Overview

The Hospital Management System uses SQL Server as the database. The schema follows a relational database design with proper foreign key relationships and constraints.

## Entity Relationship Diagram

```
┌─────────┐
│  User   │
└────┬────┘
     │
     ├───┐
     │   │
┌────▼───▼────┐  ┌──────────┐
│   Patient   │  │  Doctor   │
└────┬────────┘  └────┬─────┘
     │                │
     │                │
┌────▼────────────────▼────┐
│      Appointment         │
└──────────────────────────┘
```

## Tables

### 1. Users

Stores all user accounts in the system.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| FirstName | nvarchar(100) | NOT NULL | User's first name |
| LastName | nvarchar(100) | NOT NULL | User's last name |
| Email | nvarchar(100) | NOT NULL, UNIQUE | User's email address |
| PhoneNumber | nvarchar(20) | NOT NULL | User's phone number |
| PasswordHash | nvarchar(max) | NOT NULL | Hashed password |
| Role | nvarchar(50) | NOT NULL | User role (Admin, Doctor, Nurse, Patient) |
| IsActive | bit | NOT NULL | Account status |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

### 2. Patients

Stores patient-specific information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| UserId | int | FK, NOT NULL, UNIQUE | Reference to Users table |
| PatientNumber | nvarchar(20) | NOT NULL, UNIQUE | Unique patient identifier |
| DateOfBirth | datetime2 | NOT NULL | Patient's date of birth |
| Gender | nvarchar(10) | NOT NULL | Gender (Male, Female, Other) |
| Address | nvarchar(500) | NULL | Patient's address |
| BloodGroup | nvarchar(50) | NULL | Blood group |
| Allergies | nvarchar(1000) | NULL | Known allergies |
| EmergencyContactName | nvarchar(1000) | NULL | Emergency contact name |
| EmergencyContactPhone | nvarchar(20) | NULL | Emergency contact phone |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- UserId → Users.Id (ON DELETE RESTRICT)

### 3. Doctors

Stores doctor-specific information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| UserId | int | FK, NOT NULL, UNIQUE | Reference to Users table |
| DoctorNumber | nvarchar(50) | NOT NULL, UNIQUE | Unique doctor identifier |
| Specialization | nvarchar(100) | NOT NULL | Medical specialization |
| LicenseNumber | nvarchar(50) | NULL | Medical license number |
| PhoneNumber | nvarchar(20) | NULL | Doctor's phone number |
| Bio | nvarchar(1000) | NULL | Doctor's biography |
| ConsultationFee | decimal(18,2) | NOT NULL | Consultation fee |
| IsAvailable | bit | NOT NULL | Availability status |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- UserId → Users.Id (ON DELETE RESTRICT)

### 4. Nurses

Stores nurse-specific information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| UserId | int | FK, NOT NULL, UNIQUE | Reference to Users table |
| NurseNumber | nvarchar(50) | NOT NULL, UNIQUE | Unique nurse identifier |
| LicenseNumber | nvarchar(50) | NULL | Nursing license number |
| PhoneNumber | nvarchar(20) | NULL | Nurse's phone number |
| Department | nvarchar(100) | NULL | Department assignment |
| IsAvailable | bit | NOT NULL | Availability status |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- UserId → Users.Id (ON DELETE RESTRICT)

### 5. Appointments

Stores appointment information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| PatientId | int | FK, NOT NULL | Reference to Patients table |
| DoctorId | int | FK, NOT NULL | Reference to Doctors table |
| AppointmentDate | datetime2 | NOT NULL | Appointment date |
| AppointmentTime | nvarchar(20) | NOT NULL | Appointment time (HH:mm) |
| Status | nvarchar(50) | NOT NULL | Status (Scheduled, Completed, Cancelled, Rescheduled) |
| Reason | nvarchar(500) | NULL | Appointment reason |
| Notes | nvarchar(1000) | NULL | Additional notes |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- PatientId → Patients.Id (ON DELETE RESTRICT)
- DoctorId → Doctors.Id (ON DELETE RESTRICT)

### 6. MedicalRecords

Stores electronic health records.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| PatientId | int | FK, NOT NULL | Reference to Patients table |
| DoctorId | int | FK, NOT NULL | Reference to Doctors table |
| AppointmentId | int | FK, NULL | Reference to Appointments table |
| Diagnosis | nvarchar(200) | NOT NULL | Diagnosis |
| Symptoms | nvarchar(2000) | NULL | Symptoms |
| Treatment | nvarchar(2000) | NULL | Treatment plan |
| Notes | nvarchar(2000) | NULL | Additional notes |
| RecordDate | datetime2 | NOT NULL | Record date |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- PatientId → Patients.Id (ON DELETE RESTRICT)
- DoctorId → Doctors.Id (ON DELETE RESTRICT)
- AppointmentId → Appointments.Id (ON DELETE SET NULL)

### 7. Prescriptions

Stores prescription information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| PatientId | int | FK, NOT NULL | Reference to Patients table |
| DoctorId | int | FK, NOT NULL | Reference to Doctors table |
| AppointmentId | int | FK, NULL | Reference to Appointments table |
| MedicationName | nvarchar(200) | NOT NULL | Medication name |
| Dosage | nvarchar(500) | NULL | Dosage information |
| Instructions | nvarchar(500) | NULL | Usage instructions |
| Frequency | nvarchar(50) | NULL | Frequency (e.g., "Twice daily") |
| Quantity | int | NULL | Quantity prescribed |
| DurationDays | int | NULL | Duration in days |
| PrescribedDate | datetime2 | NOT NULL | Prescription date |
| ExpiryDate | datetime2 | NULL | Expiry date |
| IsActive | bit | NOT NULL | Active status |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- PatientId → Patients.Id (ON DELETE RESTRICT)
- DoctorId → Doctors.Id (ON DELETE RESTRICT)
- AppointmentId → Appointments.Id (ON DELETE SET NULL)

### 8. LabTestTypes

Stores available lab test types.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| Name | nvarchar(100) | NOT NULL | Test name |
| Description | nvarchar(500) | NULL | Test description |
| Price | decimal(18,2) | NOT NULL | Test price |
| Category | nvarchar(50) | NULL | Test category |
| IsActive | bit | NOT NULL | Active status |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

### 9. LabTests

Stores lab test requests and results.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| PatientId | int | FK, NOT NULL | Reference to Patients table |
| DoctorId | int | FK, NOT NULL | Reference to Doctors table |
| LabTestTypeId | int | FK, NOT NULL | Reference to LabTestTypes table |
| AppointmentId | int | FK, NULL | Reference to Appointments table |
| TestNumber | nvarchar(50) | NOT NULL, UNIQUE | Unique test identifier |
| Status | nvarchar(50) | NOT NULL | Status (Pending, InProgress, Completed, Cancelled) |
| Results | nvarchar(2000) | NULL | Test results |
| Notes | nvarchar(500) | NULL | Additional notes |
| RequestedDate | datetime2 | NOT NULL | Request date |
| CompletedDate | datetime2 | NULL | Completion date |
| Price | decimal(18,2) | NOT NULL | Test price |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- PatientId → Patients.Id (ON DELETE RESTRICT)
- DoctorId → Doctors.Id (ON DELETE RESTRICT)
- LabTestTypeId → LabTestTypes.Id (ON DELETE RESTRICT)
- AppointmentId → Appointments.Id (ON DELETE SET NULL)

### 10. Invoices

Stores billing invoices.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| PatientId | int | FK, NOT NULL | Reference to Patients table |
| InvoiceNumber | nvarchar(50) | NOT NULL, UNIQUE | Unique invoice identifier |
| TotalAmount | decimal(18,2) | NOT NULL | Total amount |
| DiscountAmount | decimal(18,2) | NULL | Discount amount |
| FinalAmount | decimal(18,2) | NOT NULL | Final amount after discount |
| Status | nvarchar(50) | NOT NULL | Status (Pending, Paid, PartiallyPaid, Cancelled) |
| Notes | nvarchar(500) | NULL | Additional notes |
| InvoiceDate | datetime2 | NOT NULL | Invoice date |
| DueDate | datetime2 | NULL | Due date |
| PaidDate | datetime2 | NULL | Payment date |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- PatientId → Patients.Id (ON DELETE RESTRICT)

### 11. InvoiceItems

Stores invoice line items.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| InvoiceId | int | FK, NOT NULL | Reference to Invoices table |
| ItemName | nvarchar(200) | NOT NULL | Item name |
| ItemType | nvarchar(50) | NULL | Item type (Consultation, LabTest, etc.) |
| ItemReferenceId | int | NULL | Reference to related entity |
| Quantity | decimal(18,2) | NOT NULL | Quantity |
| UnitPrice | decimal(18,2) | NOT NULL | Unit price |
| TotalPrice | decimal(18,2) | NOT NULL | Total price |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |

**Foreign Keys:**
- InvoiceId → Invoices.Id (ON DELETE CASCADE)

### 12. Rooms

Stores hospital room information.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| RoomNumber | nvarchar(50) | NOT NULL, UNIQUE | Room number |
| RoomType | nvarchar(50) | NOT NULL | Room type (General, ICU, Private, etc.) |
| Floor | nvarchar(100) | NULL | Floor location |
| Description | nvarchar(500) | NULL | Room description |
| PricePerDay | decimal(18,2) | NOT NULL | Daily room charge |
| IsAvailable | bit | NOT NULL | Availability status |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

### 13. Beds

Stores bed information within rooms.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| RoomId | int | FK, NOT NULL | Reference to Rooms table |
| BedNumber | nvarchar(50) | NOT NULL | Bed number within room |
| Description | nvarchar(500) | NULL | Bed description |
| IsOccupied | bit | NOT NULL | Occupancy status |
| IsAvailable | bit | NOT NULL | Availability status |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- RoomId → Rooms.Id (ON DELETE CASCADE)
- Unique Constraint: (RoomId, BedNumber)

### 14. Admissions

Stores patient admission records.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| PatientId | int | FK, NOT NULL | Reference to Patients table |
| BedId | int | FK, NOT NULL | Reference to Beds table |
| DoctorId | int | FK, NULL | Reference to Doctors table |
| NurseId | int | FK, NULL | Reference to Nurses table |
| AdmissionNumber | nvarchar(50) | NOT NULL, UNIQUE | Unique admission identifier |
| AdmissionDate | datetime2 | NOT NULL | Admission date |
| DischargeDate | datetime2 | NULL | Discharge date |
| Status | nvarchar(50) | NOT NULL | Status (Admitted, Discharged, Transferred) |
| Reason | nvarchar(1000) | NULL | Admission reason |
| Notes | nvarchar(2000) | NULL | Additional notes |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- PatientId → Patients.Id (ON DELETE RESTRICT)
- BedId → Beds.Id (ON DELETE RESTRICT)
- DoctorId → Doctors.Id (ON DELETE SET NULL)
- NurseId → Nurses.Id (ON DELETE SET NULL)

### 15. DoctorSchedules

Stores doctor availability schedules.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PK, Identity | Primary key |
| DoctorId | int | FK, NOT NULL | Reference to Doctors table |
| DayOfWeek | nvarchar(20) | NOT NULL | Day of week (Monday, Tuesday, etc.) |
| StartTime | nvarchar(20) | NOT NULL | Start time (HH:mm) |
| EndTime | nvarchar(20) | NOT NULL | End time (HH:mm) |
| IsAvailable | bit | NOT NULL | Availability status |
| CreatedAt | datetime2 | NOT NULL | Creation timestamp |
| UpdatedAt | datetime2 | NULL | Last update timestamp |

**Foreign Keys:**
- DoctorId → Doctors.Id (ON DELETE CASCADE)

## Indexes

The following indexes are created for performance optimization:

1. **Users.Email** - Unique index for fast email lookups
2. **Patients.PatientNumber** - Unique index
3. **Doctors.DoctorNumber** - Unique index
4. **Nurses.NurseNumber** - Unique index
5. **Invoices.InvoiceNumber** - Unique index
6. **LabTests.TestNumber** - Unique index
7. **Admissions.AdmissionNumber** - Unique index
8. **Rooms.RoomNumber** - Unique index
9. **Beds (RoomId, BedNumber)** - Composite unique index

## Relationships Summary

- **One-to-One**: User → Patient, User → Doctor, User → Nurse
- **One-to-Many**: Patient → Appointments, Patient → MedicalRecords, Patient → Prescriptions, Patient → LabTests, Patient → Invoices, Patient → Admissions
- **One-to-Many**: Doctor → Appointments, Doctor → MedicalRecords, Doctor → Prescriptions, Doctor → LabTests, Doctor → DoctorSchedules
- **One-to-Many**: Room → Beds
- **One-to-Many**: Bed → Admissions
- **One-to-Many**: Invoice → InvoiceItems

