# System Architecture

## Overview

The Hospital Management System is built using Clean Architecture principles, ensuring separation of concerns, maintainability, and testability.

## Architecture Layers

### 1. Domain Layer (HMS.Domain)

**Purpose**: Contains core business entities and interfaces.

**Components**:
- Entities (User, Patient, Doctor, Appointment, etc.)
- Interfaces (IRepository, IUnitOfWork, IJwtService, etc.)
- Domain-specific business rules

**Dependencies**: None (Pure domain logic)

### 2. Application Layer (HMS.Application)

**Purpose**: Contains application use cases and business logic.

**Components**:
- DTOs (Data Transfer Objects)
- Services (Business logic)
- Validators (FluentValidation)
- Mappings (AutoMapper profiles)

**Dependencies**: Domain layer only

### 3. Infrastructure Layer (HMS.Infrastructure)

**Purpose**: Contains data access and external service implementations.

**Components**:
- DbContext (Entity Framework)
- Repositories (Data access)
- Unit of Work pattern
- External services (JWT, Email, PDF)

**Dependencies**: Domain and Application layers

### 4. Presentation Layer (HMS.API)

**Purpose**: Contains API controllers and configuration.

**Components**:
- Controllers (REST API endpoints)
- Middleware (Authentication, CORS)
- Configuration (Dependency Injection, Swagger)

**Dependencies**: All other layers

## Design Patterns

### Repository Pattern
- Abstracts data access logic
- Provides testability
- Centralizes data access code

### Unit of Work Pattern
- Manages database transactions
- Ensures data consistency
- Coordinates multiple repositories

### Dependency Injection
- Loose coupling
- Testability
- Maintainability

### DTO Pattern
- Separates API contracts from domain models
- Prevents over-posting
- Versioning support

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT Bearer Tokens
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: Next.js 14
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **State Management**: Zustand
- **Data Fetching**: React Query
- **HTTP Client**: Axios
- **Forms**: React Hook Form

## Security Architecture

### Authentication
- JWT-based authentication
- Token expiration (configurable)
- Secure password hashing (SHA256)

### Authorization
- Role-based access control (RBAC)
- Claims-based authorization
- Resource-level authorization

### Data Protection
- HTTPS enforcement
- SQL injection prevention (EF Core parameterized queries)
- CORS configuration
- Input validation

## Scalability Considerations

### Horizontal Scaling
- Stateless API design
- Database connection pooling
- Load balancer ready

### Performance Optimization
- Async/await throughout
- Database indexing
- Response caching (can be added)
- Pagination support (can be added)

## Future Enhancements

1. **Caching Layer**: Redis for session and data caching
2. **Message Queue**: RabbitMQ/Azure Service Bus for async operations
3. **Search**: Elasticsearch for advanced search
4. **Real-time**: SignalR for real-time notifications
5. **Microservices**: Split into microservices if needed
6. **API Gateway**: Add API gateway for routing and rate limiting

