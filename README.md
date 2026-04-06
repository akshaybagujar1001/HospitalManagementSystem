# Hospital Management System (HMS)

A comprehensive, enterprise-level Hospital Management System built with modern technologies following Clean Architecture principles.

## 🏥 System Overview

This Hospital Management System provides a complete solution for managing hospital operations including:

- **Patient Management** - Complete patient lifecycle management
- **Doctor Management** - Doctor profiles, specializations, and schedules
- **Nurse Management** - Nurse assignments and schedules
- **Appointments** - Scheduling, cancellation, and rescheduling
- **Medical Records (EHR)** - Electronic Health Records management
- **Prescriptions** - Digital prescription management
- **Laboratory Tests** - Test requests and results management
- **Billing & Invoices** - Automated billing with PDF generation
- **Room & Bed Management** - Hospital room and bed allocation
- **Authentication & Authorization** - JWT-based security with role-based access
- **Email Notifications** - Automated email notifications
- **Dashboards & Analytics** - Role-specific dashboards with analytics

## 🛠️ Technology Stack

### Backend
- **ASP.NET Core Web API** (.NET 8)
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT Authentication** - Security
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation

### Frontend
- **Next.js 14** - React framework
- **TypeScript** - Type safety
- **Tailwind CSS** - Styling
- **Axios** - HTTP client
- **React Query** - Data fetching
- **React Hook Form** - Form management
- **Zustand** - State management

### Database
- **SQL Server** - Relational database
- **Entity Framework Core Migrations** - Database versioning

## 📁 Project Structur

```
Hospital-Management-System/
├── Backend/
│   ├── HMS.API/                    # Web API layer
│   ├── HMS.Application/            # Application layer (use cases)
│   ├── HMS.Domain/                 # Domain layer (entities, interfaces)
│   ├── HMS.Infrastructure/         # Infrastructure layer (data access, external services)
│   └── HMS.Shared/                 # Shared utilities
├── Frontend/
│   ├── src/
│   │   ├── app/                    # Next.js app router
│   │   ├── components/             # React components
│   │   ├── lib/                    # Utilities and configurations
│   │   └── hooks/                  # Custom React hooks
│   └── public/                     # Static assets
└── Documentation/
    ├── UML-Diagrams/               # System diagrams
    ├── Database/                   # Database schema and ERD
    └── API-Documentation/          # API documentation
```

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server 2019 or later
- Node.js 18+ and npm/yarn
- Visual Studio 2022 or VS Code

### Backend Setup

1. Navigate to the backend directory:
```bash
cd Backend/HMS.API
```

2. Update connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HMSDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Run migrations:
```bash
dotnet ef database update --project ../HMS.Infrastructure --startup-project .
```

4. Run the API:
```bash
dotnet run
```

The API will be available at `https://localhost:7001` or `http://localhost:5001`

### Frontend Setup

1. Navigate to the frontend directory:
```bash
cd Frontend
```

2. Install dependencies:
```bash
npm install
```

3. Create `.env.local` file:
```env
NEXT_PUBLIC_API_URL=http://localhost:5001/api
```

4. Run the development server:
```bash
npm run dev
```

The frontend will be available at `http://localhost:3000`


