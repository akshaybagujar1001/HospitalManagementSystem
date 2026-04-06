# API Documentation

## Base URL

```
http://localhost:5001/api
```

## Authentication

All protected endpoints require JWT authentication. Include the token in the Authorization header:

```
Authorization: Bearer <token>
```

## Endpoints

### Authentication

#### Register
- **POST** `/api/auth/register`
- **Description**: Register a new user
- **Authentication**: Not required
- **Request Body**:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "1234567890",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "role": "Patient"
}
```
- **Response**: `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "john.doe@example.com",
  "role": "Patient",
  "firstName": "John",
  "lastName": "Doe",
  "userId": 1
}
```

#### Login
- **POST** `/api/auth/login`
- **Description**: Authenticate user and get JWT token
- **Authentication**: Not required
- **Request Body**:
```json
{
  "email": "john.doe@example.com",
  "password": "Password123!"
}
```
- **Response**: `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "john.doe@example.com",
  "role": "Patient",
  "firstName": "John",
  "lastName": "Doe",
  "userId": 1
}
```

### Patients

#### Get All Patients
- **GET** `/api/patients`
- **Description**: Get all patients (Admin, Doctor, Nurse only)
- **Authentication**: Required
- **Authorization**: Admin, Doctor, Nurse
- **Response**: `200 OK`
```json
[
  {
    "id": 1,
    "userId": 1,
    "patientNumber": "PAT-20240101-001",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "1234567890",
    "dateOfBirth": "1990-01-01T00:00:00",
    "gender": "Male",
    "address": "123 Main St",
    "bloodGroup": "O+",
    "allergies": "None",
    "emergencyContactName": "Jane Doe",
    "emergencyContactPhone": "9876543210",
    "createdAt": "2024-01-01T00:00:00"
  }
]
```

#### Get Patient by ID
- **GET** `/api/patients/{id}`
- **Description**: Get patient details by ID
- **Authentication**: Required
- **Authorization**: Patient can only view own record
- **Response**: `200 OK`
```json
{
  "id": 1,
  "userId": 1,
  "patientNumber": "PAT-20240101-001",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "1234567890",
  "dateOfBirth": "1990-01-01T00:00:00",
  "gender": "Male",
  "address": "123 Main St",
  "bloodGroup": "O+",
  "allergies": "None",
  "emergencyContactName": "Jane Doe",
  "emergencyContactPhone": "9876543210",
  "createdAt": "2024-01-01T00:00:00"
}
```

#### Create Patient
- **POST** `/api/patients`
- **Description**: Create a new patient (Admin only)
- **Authentication**: Required
- **Authorization**: Admin
- **Request Body**:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "1234567890",
  "dateOfBirth": "1990-01-01T00:00:00",
  "gender": "Male",
  "address": "123 Main St",
  "bloodGroup": "O+",
  "allergies": "None",
  "emergencyContactName": "Jane Doe",
  "emergencyContactPhone": "9876543210"
}
```
- **Response**: `201 Created`

#### Update Patient
- **PUT** `/api/patients/{id}`
- **Description**: Update patient information
- **Authentication**: Required
- **Authorization**: Patient can only update own record
- **Request Body**: Same as Create Patient
- **Response**: `200 OK`

#### Get Patient Appointments
- **GET** `/api/patients/{id}/appointments`
- **Description**: Get all appointments for a patient
- **Authentication**: Required
- **Response**: `200 OK`
```json
[
  {
    "id": 1,
    "patientId": 1,
    "patientName": "John Doe",
    "doctorId": 1,
    "doctorName": "Dr. Smith",
    "doctorSpecialization": "Cardiology",
    "appointmentDate": "2024-01-15T00:00:00",
    "appointmentTime": "10:00",
    "status": "Scheduled",
    "reason": "Regular checkup",
    "notes": null,
    "createdAt": "2024-01-01T00:00:00"
  }
]
```

#### Get Patient Medical Records
- **GET** `/api/patients/{id}/medical-records`
- **Description**: Get medical records for a patient
- **Authentication**: Required
- **Response**: `200 OK`

#### Get Patient Invoices
- **GET** `/api/patients/{id}/invoices`
- **Description**: Get all invoices for a patient
- **Authentication**: Required
- **Response**: `200 OK`

### Doctors

#### Get All Doctors
- **GET** `/api/doctors`
- **Description**: Get all available doctors
- **Authentication**: Required
- **Query Parameters**:
  - `specialization` (optional): Filter by specialization
- **Response**: `200 OK`
```json
[
  {
    "id": 1,
    "userId": 2,
    "doctorNumber": "DOC-20240101-001",
    "firstName": "John",
    "lastName": "Doctor",
    "email": "doctor@hms.com",
    "phoneNumber": "1234567891",
    "specialization": "Cardiology",
    "licenseNumber": "LIC-12345",
    "bio": "Experienced cardiologist",
    "consultationFee": 500.00,
    "isAvailable": true
  }
]
```

#### Get Doctor by ID
- **GET** `/api/doctors/{id}`
- **Description**: Get doctor details by ID
- **Authentication**: Required
- **Response**: `200 OK`

#### Get Doctor Appointments
- **GET** `/api/doctors/{id}/appointments`
- **Description**: Get all appointments for a doctor
- **Authentication**: Required
- **Response**: `200 OK`

### Appointments

#### Get All Appointments
- **GET** `/api/appointments`
- **Description**: Get appointments (filtered by user role)
- **Authentication**: Required
- **Query Parameters**:
  - `patientId` (optional): Filter by patient ID
  - `doctorId` (optional): Filter by doctor ID
- **Response**: `200 OK`

#### Get Appointment by ID
- **GET** `/api/appointments/{id}`
- **Description**: Get appointment details by ID
- **Authentication**: Required
- **Response**: `200 OK`

#### Create Appointment
- **POST** `/api/appointments`
- **Description**: Create a new appointment
- **Authentication**: Required
- **Request Body**:
```json
{
  "patientId": 1,
  "doctorId": 1,
  "appointmentDate": "2024-01-15T00:00:00",
  "appointmentTime": "10:00",
  "reason": "Regular checkup"
}
```
- **Response**: `201 Created`

#### Cancel Appointment
- **PUT** `/api/appointments/{id}/cancel`
- **Description**: Cancel an appointment
- **Authentication**: Required
- **Response**: `200 OK`
```json
{
  "message": "Appointment cancelled successfully"
}
```

#### Reschedule Appointment
- **PUT** `/api/appointments/{id}/reschedule`
- **Description**: Reschedule an appointment
- **Authentication**: Required
- **Request Body**: Same as Create Appointment
- **Response**: `200 OK`

#### Complete Appointment
- **PUT** `/api/appointments/{id}/complete`
- **Description**: Mark appointment as completed (Doctor, Admin only)
- **Authentication**: Required
- **Authorization**: Doctor, Admin
- **Response**: `200 OK`

### Invoices

#### Get All Invoices
- **GET** `/api/invoices`
- **Description**: Get all invoices (Admin only)
- **Authentication**: Required
- **Authorization**: Admin
- **Response**: `200 OK`

#### Get Invoice by ID
- **GET** `/api/invoices/{id}`
- **Description**: Get invoice details by ID
- **Authentication**: Required
- **Response**: `200 OK`
```json
{
  "id": 1,
  "patientId": 1,
  "patientName": "John Doe",
  "invoiceNumber": "INV-20240101-001",
  "totalAmount": 1000.00,
  "discountAmount": 0.00,
  "finalAmount": 1000.00,
  "status": "Pending",
  "invoiceDate": "2024-01-01T00:00:00",
  "dueDate": "2024-01-15T00:00:00",
  "paidDate": null,
  "items": [
    {
      "id": 1,
      "itemName": "Consultation",
      "itemType": "Consultation",
      "quantity": 1,
      "unitPrice": 500.00,
      "totalPrice": 500.00
    }
  ]
}
```

#### Get Invoice PDF
- **GET** `/api/invoices/{id}/pdf`
- **Description**: Download invoice as PDF
- **Authentication**: Required
- **Response**: `200 OK` (PDF file)

#### Mark Invoice as Paid
- **PUT** `/api/invoices/{id}/pay`
- **Description**: Mark invoice as paid
- **Authentication**: Required
- **Response**: `200 OK`
```json
{
  "message": "Invoice marked as paid"
}
```

## Error Responses

### 400 Bad Request
```json
{
  "message": "Validation error message"
}
```

### 401 Unauthorized
```json
{
  "message": "Invalid email or password"
}
```

### 403 Forbidden
```json
{
  "message": "Access denied"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred"
}
```

## Status Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required or failed
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

