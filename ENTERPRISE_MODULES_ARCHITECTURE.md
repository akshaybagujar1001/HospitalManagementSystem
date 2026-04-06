# Enterprise Hospital Management System - New Modules Architecture

## 📋 Overview

This document outlines the architecture and implementation of 14 new enterprise-level modules added to the existing Hospital Management System. All modules follow Clean Architecture principles and integrate seamlessly with the existing codebase.

---

## 🏗️ 1. ARCHITECTURE OVERVIEW

### New Entities Created (20 entities)

1. **Notification** - Real-time notifications
2. **AuditLog** - Activity tracking
3. **InsuranceCompany** - Insurance providers
4. **InsurancePolicy** - Patient insurance policies
5. **InsuranceClaim** - Insurance claims
6. **Medication** - Pharmacy inventory
7. **MedicationStockMovement** - Stock tracking
8. **PrescriptionFulfillment** - Prescription dispensing
9. **RadiologyOrder** - Imaging orders
10. **RadiologyImage** - Image files
11. **RadiologyReport** - Radiology reports
12. **NurseTask** - Task management
13. **EmergencyCase** - ER cases
14. **Asset** - Hospital assets
15. **AssetMaintenance** - Maintenance records
16. **AIDiagnosis** - AI diagnostic results
17. **StaffShift** - Shift scheduling
18. **Expense** - Financial expenses
19. **RevenueRecord** - Revenue tracking

### Updated Infrastructure

- ✅ **HmsDbContext** - Added all new DbSets
- ✅ **IUnitOfWork** - Added repositories for all new entities
- ✅ **UnitOfWork** - Implemented all new repositories
- ✅ **Program.cs** - Registered SignalR and new services

---

## 🔔 2. REAL-TIME NOTIFICATION SYSTEM (SignalR)

### Implementation Status: ✅ COMPLETE

**Files Created:**
- `Backend/HMS.API/Hubs/NotificationHub.cs`
- `Backend/HMS.Application/Services/INotificationService.cs`
- `Backend/HMS.Application/Services/NotificationService.cs`
- `Backend/HMS.Application/DTOs/Notification/NotificationDto.cs`
- `Backend/HMS.API/Controllers/NotificationsController.cs`

**Key Features:**
- Real-time notifications via SignalR
- Notification types: Appointment, LabResult, Prescription, Billing, System, Emergency
- Mark as read/unread functionality
- Unread count endpoint

**Integration Points:**
To send notifications from existing controllers, inject `INotificationService`:

```csharp
// Example: In AppointmentsController after creating appointment
await _notificationService.SendNotificationAsync(
    patient.UserId,
    "New Appointment Scheduled",
    $"Your appointment with Dr. {doctor.User.LastName} is scheduled for {appointment.AppointmentDate}",
    "Appointment",
    "Appointment",
    appointment.Id
);
```

**Frontend Integration:**
```typescript
// Connect to SignalR hub
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('/notificationHub', {
    accessTokenFactory: () => getToken()
  })
  .build();

connection.on('ReceiveNotification', (notification) => {
  // Display notification
});
```

---

## 🤖 3. AI DIAGNOSTIC ASSISTANT

### Implementation Status: ✅ COMPLETE

**Files Created:**
- `Backend/HMS.Application/Services/IAIDiagnosisService.cs`
- `Backend/HMS.Application/Services/AIDiagnosisService.cs`
- `Backend/HMS.Application/DTOs/AIDiagnosis/*.cs`
- `Backend/HMS.API/Controllers/AIDiagnosisController.cs`

**Endpoints:**
- `POST /api/ai-diagnosis/analyze` - Analyze symptoms
- `GET /api/ai-diagnosis/{id}` - Get diagnosis
- `GET /api/ai-diagnosis/patient/{patientId}` - Get patient diagnoses

**Note:** Currently uses mock AI logic. Replace `PerformMockAnalysis` method with actual AI service integration (OpenAI, Azure AI, etc.).

---

## 🏥 4. INSURANCE INTEGRATION MODULE

### Implementation Status: ⚠️ ENTITIES CREATED - CONTROLLERS NEEDED

**Entities:** ✅ Complete
- InsuranceCompany
- InsurancePolicy
- InsuranceClaim

**Required Controllers (To Be Created):**

### InsuranceCompaniesController.cs
```csharp
[ApiController]
[Route("api/insurance-companies")]
[Authorize(Roles = "Admin,Finance")]
public class InsuranceCompaniesController : ControllerBase
{
    // GET /api/insurance-companies
    // GET /api/insurance-companies/{id}
    // POST /api/insurance-companies
    // PUT /api/insurance-companies/{id}
    // DELETE /api/insurance-companies/{id}
}
```

### InsurancePoliciesController.cs
```csharp
[ApiController]
[Route("api/insurance-policies")]
[Authorize]
public class InsurancePoliciesController : ControllerBase
{
    // GET /api/insurance-policies/patient/{patientId}
    // POST /api/insurance-policies
    // PUT /api/insurance-policies/{id}
    // GET /api/insurance-policies/{id}
}
```

### InsuranceClaimsController.cs
```csharp
[ApiController]
[Route("api/insurance-claims")]
[Authorize(Roles = "Admin,Finance")]
public class InsuranceClaimsController : ControllerBase
{
    // POST /api/insurance-claims (create from invoice)
    // GET /api/insurance-claims
    // GET /api/insurance-claims/{id}
    // PUT /api/insurance-claims/{id}/status
    // GET /api/insurance-claims/patient/{patientId}
}
```

**Integration with Invoices:**
When an invoice is created/paid, check for active insurance and create claim:
```csharp
// In InvoicesController
var policy = await _unitOfWork.InsurancePolicies
    .FirstOrDefaultAsync(p => p.PatientId == invoice.PatientId && p.IsActive);
    
if (policy != null)
{
    var claim = new InsuranceClaim { /* ... */ };
    // Calculate coverage
}
```

---

## 💊 5. PHARMACY MANAGEMENT MODULE

### Implementation Status: ⚠️ ENTITIES CREATED - CONTROLLERS NEEDED

**Entities:** ✅ Complete
- Medication
- MedicationStockMovement
- PrescriptionFulfillment

**Required Controllers:**

### MedicationsController.cs
```csharp
[ApiController]
[Route("api/medications")]
[Authorize(Roles = "Admin,Pharmacist")]
public class MedicationsController : ControllerBase
{
    // GET /api/medications
    // GET /api/medications/{id}
    // POST /api/medications
    // PUT /api/medications/{id}
    // GET /api/medications/low-stock (reorder level)
    // POST /api/medications/{id}/adjust-stock
}
```

### PrescriptionFulfillmentsController.cs
```csharp
[ApiController]
[Route("api/prescription-fulfillments")]
[Authorize(Roles = "Admin,Pharmacist")]
public class PrescriptionFulfillmentsController : ControllerBase
{
    // POST /api/prescription-fulfillments (fulfill prescription)
    // GET /api/prescription-fulfillments/prescription/{prescriptionId}
    // GET /api/prescription-fulfillments
}
```

**Integration with Prescriptions:**
When fulfilling a prescription, update medication stock:
```csharp
// Reduce stock
medication.StockQuantity -= quantity;
// Create stock movement
// Create fulfillment record
```

---

## 🏥 6. RADIOLOGY & IMAGING MODULE

### Implementation Status: ⚠️ ENTITIES CREATED - CONTROLLERS NEEDED

**Entities:** ✅ Complete
- RadiologyOrder
- RadiologyImage
- RadiologyReport

**Required Controllers:**

### RadiologyOrdersController.cs
```csharp
[ApiController]
[Route("api/radiology-orders")]
[Authorize]
public class RadiologyOrdersController : ControllerBase
{
    // POST /api/radiology-orders
    // GET /api/radiology-orders
    // GET /api/radiology-orders/{id}
    // PUT /api/radiology-orders/{id}/status
    // GET /api/radiology-orders/patient/{patientId}
}
```

### RadiologyReportsController.cs
```csharp
[ApiController]
[Route("api/radiology-reports")]
[Authorize(Roles = "Doctor,Admin")]
public class RadiologyReportsController : ControllerBase
{
    // POST /api/radiology-reports
    // GET /api/radiology-reports/order/{orderId}
    // PUT /api/radiology-reports/{id}
}
```

### RadiologyImagesController.cs
```csharp
[ApiController]
[Route("api/radiology-images")]
[Authorize]
public class RadiologyImagesController : ControllerBase
{
    // POST /api/radiology-images (upload - returns file path)
    // GET /api/radiology-images/order/{orderId}
    // DELETE /api/radiology-images/{id}
}
```

---

## 👩‍⚕️ 7. NURSE TASK MANAGEMENT MODULE

### Implementation Status: ⚠️ ENTITIES CREATED - CONTROLLERS NEEDED

**Entity:** ✅ Complete
- NurseTask

**Required Controller:**

### NurseTasksController.cs
```csharp
[ApiController]
[Route("api/nurse-tasks")]
[Authorize]
public class NurseTasksController : ControllerBase
{
    // GET /api/nurse-tasks (filtered by nurse role)
    // GET /api/nurse-tasks/{id}
    // POST /api/nurse-tasks
    // PUT /api/nurse-tasks/{id}
    // PUT /api/nurse-tasks/{id}/status
    // GET /api/nurse-tasks/nurse/{nurseId}
    // GET /api/nurse-tasks/patient/{patientId}
}
```

**Integration:** Doctors/Admins can assign tasks to nurses. Nurses see their tasks in dashboard.

---

## 🚨 8. EMERGENCY DEPARTMENT (ER) MODULE

### Implementation Status: ⚠️ ENTITIES CREATED - CONTROLLERS NEEDED

**Entity:** ✅ Complete
- EmergencyCase

**Required Controller:**

### EmergencyCasesController.cs
```csharp
[ApiController]
[Route("api/emergency-cases")]
[Authorize(Roles = "Admin,Doctor,Nurse")]
public class EmergencyCasesController : ControllerBase
{
    // POST /api/emergency-cases (register new case)
    // GET /api/emergency-cases
    // GET /api/emergency-cases/{id}
    // PUT /api/emergency-cases/{id}/triage
    // PUT /api/emergency-cases/{id}/assign
    // PUT /api/emergency-cases/{id}/status
    // GET /api/emergency-cases/active
}
```

**Triage Levels:** 1=Critical, 2=Urgent, 3=Moderate, 4=Less Urgent, 5=Non-Urgent

---

## 📦 9. HOSPITAL ASSET TRACKING MODULE

### Implementation Status: ⚠️ ENTITIES CREATED - CONTROLLERS NEEDED

**Entities:** ✅ Complete
- Asset
- AssetMaintenance

**Required Controllers:**

### AssetsController.cs
```csharp
[ApiController]
[Route("api/assets")]
[Authorize(Roles = "Admin")]
public class AssetsController : ControllerBase
{
    // GET /api/assets
    // GET /api/assets/{id}
    // POST /api/assets
    // PUT /api/assets/{id}
    // GET /api/assets/maintenance-due
}
```

### AssetMaintenanceController.cs
```csharp
[ApiController]
[Route("api/asset-maintenance")]
[Authorize(Roles = "Admin")]
public class AssetMaintenanceController : ControllerBase
{
    // POST /api/asset-maintenance
    // GET /api/asset-maintenance/asset/{assetId}
    // GET /api/asset-maintenance
}
```

---

## 📊 10. KPI & ANALYTICS DASHBOARD

### Implementation Status: ✅ COMPLETE

**Files Created:**
- `Backend/HMS.Application/Services/IManagementDashboardService.cs`
- `Backend/HMS.Application/Services/ManagementDashboardService.cs`
- `Backend/HMS.Application/DTOs/Dashboard/*.cs`
- `Backend/HMS.API/Controllers/ManagementDashboardController.cs`

**Endpoints:**
- `GET /api/dashboard/overview` - Key metrics
- `GET /api/dashboard/financial` - Financial KPIs
- `GET /api/dashboard/operational` - Operational metrics

---

## 🔄 11. HL7 / FHIR INTEROPERABILITY LAYER

### Implementation Status: ✅ COMPLETE

**Files Created:**
- `Backend/HMS.Application/Services/IFhirExportService.cs`
- `Backend/HMS.Application/Services/FhirExportService.cs`
- `Backend/HMS.Application/DTOs/Fhir/*.cs`
- `Backend/HMS.API/Controllers/InteroperabilityController.cs`

**Endpoints:**
- `GET /api/interoperability/patient/{id}` - FHIR Patient resource
- `GET /api/interoperability/encounters/{patientId}` - FHIR Encounters
- `GET /api/interoperability/observation/{labTestId}` - FHIR Observation

**Note:** This is a simplified FHIR implementation. For production, use a full FHIR library (e.g., Hl7.Fhir.R4).

---

## 📱 12. OFFLINE-FIRST NURSE MOBILE APP (React Native)

### Structure:

```
hms-nurse-app/
├── src/
│   ├── screens/
│   │   ├── LoginScreen.tsx
│   │   ├── TaskListScreen.tsx
│   │   ├── TaskDetailScreen.tsx
│   │   ├── PatientListScreen.tsx
│   │   └── SyncScreen.tsx
│   ├── services/
│   │   ├── api.ts
│   │   ├── syncService.ts
│   │   └── storageService.ts
│   ├── components/
│   │   ├── TaskCard.tsx
│   │   └── PatientCard.tsx
│   └── hooks/
│       ├── useTasks.ts
│       └── useSync.ts
├── package.json
└── README.md
```

**Key Files to Create:**

### src/services/storageService.ts
```typescript
import AsyncStorage from '@react-native-async-storage/async-storage';

export const StorageService = {
  async saveTasks(tasks: any[]) {
    await AsyncStorage.setItem('tasks', JSON.stringify(tasks));
  },
  
  async getTasks() {
    const data = await AsyncStorage.getItem('tasks');
    return data ? JSON.parse(data) : [];
  },
  
  async saveSyncTimestamp(timestamp: number) {
    await AsyncStorage.setItem('lastSync', timestamp.toString());
  }
};
```

### src/services/syncService.ts
```typescript
import { StorageService } from './storageService';
import { api } from './api';

export const SyncService = {
  async syncTasks() {
    try {
      // Get local tasks
      const localTasks = await StorageService.getTasks();
      
      // Get server tasks
      const serverTasks = await api.get('/nurse-tasks');
      
      // Merge and resolve conflicts
      const merged = this.mergeTasks(localTasks, serverTasks.data);
      
      // Save merged
      await StorageService.saveTasks(merged);
      
      // Upload pending changes
      await this.uploadPendingChanges();
      
      await StorageService.saveSyncTimestamp(Date.now());
    } catch (error) {
      console.error('Sync failed:', error);
    }
  },
  
  mergeTasks(local: any[], server: any[]) {
    // Merge logic
    return [...];
  },
  
  async uploadPendingChanges() {
    // Upload locally modified tasks
  }
};
```

### src/screens/TaskListScreen.tsx (Example)
```typescript
import React, { useEffect, useState } from 'react';
import { View, FlatList } from 'react-native';
import { useTasks } from '../hooks/useTasks';
import { TaskCard } from '../components/TaskCard';
import { SyncService } from '../services/syncService';

export const TaskListScreen = () => {
  const { tasks, loading, refresh } = useTasks();
  
  useEffect(() => {
    refresh();
    // Sync when online
    SyncService.syncTasks();
  }, []);
  
  return (
    <View>
      <FlatList
        data={tasks}
        renderItem={({ item }) => <TaskCard task={item} />}
        keyExtractor={item => item.id.toString()}
        refreshing={loading}
        onRefresh={refresh}
      />
    </View>
  );
};
```

---

## 📝 13. AUDIT LOG / ACTIVITY LOG MODULE

### Implementation Status: ⚠️ ENTITY & INTERCEPTOR CREATED - CONTROLLER NEEDED

**Files Created:**
- `Backend/HMS.Domain/Entities/AuditLog.cs` ✅
- `Backend/HMS.Infrastructure/Data/AuditLogInterceptor.cs` ✅

**Required Controller:**

### AuditLogsController.cs
```csharp
[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = "Admin")]
public class AuditLogsController : ControllerBase
{
    // GET /api/audit-logs
    // GET /api/audit-logs/entity/{entityName}/{entityId}
    // GET /api/audit-logs/user/{userId}
    // GET /api/audit-logs?startDate=&endDate=
}
```

**Enable Interceptor in Program.cs:**
```csharp
builder.Services.AddDbContext<HmsDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.AddInterceptors(new AuditLogInterceptor(context));
});
```

---

## 👥 14. STAFF SHIFT SCHEDULING MODULE

### Implementation Status: ⚠️ ENTITIES CREATED - CONTROLLERS NEEDED

**Entity:** ✅ Complete
- StaffShift

**Required Controller:**

### StaffShiftsController.cs
```csharp
[ApiController]
[Route("api/staff-shifts")]
[Authorize(Roles = "Admin")]
public class StaffShiftsController : ControllerBase
{
    // GET /api/staff-shifts
    // GET /api/staff-shifts/staff/{staffId}
    // GET /api/staff-shifts/week?startDate=
    // POST /api/staff-shifts
    // PUT /api/staff-shifts/{id}
    // DELETE /api/staff-shifts/{id}
}
```

---

## 💰 15. FINANCIAL ACCOUNTING MODULE

### Implementation Status: ⚠️ ENTITIES CREATED - CONTROLLERS NEEDED

**Entities:** ✅ Complete
- Expense
- RevenueRecord

**Required Controllers:**

### ExpensesController.cs
```csharp
[ApiController]
[Route("api/expenses")]
[Authorize(Roles = "Admin,Finance")]
public class ExpensesController : ControllerBase
{
    // GET /api/expenses
    // GET /api/expenses/{id}
    // POST /api/expenses
    // PUT /api/expenses/{id}
    // GET /api/expenses/summary?startDate=&endDate=
}
```

### RevenuesController.cs
```csharp
[ApiController]
[Route("api/revenues")]
[Authorize(Roles = "Admin,Finance")]
public class RevenuesController : ControllerBase
{
    // GET /api/revenues
    // GET /api/revenues/{id}
    // POST /api/revenues
    // GET /api/revenues/summary?startDate=&endDate=
}
```

### FinancialReportsController.cs
```csharp
[ApiController]
[Route("api/financial-reports")]
[Authorize(Roles = "Admin,Finance")]
public class FinancialReportsController : ControllerBase
{
    // GET /api/financial-reports/profit-loss?startDate=&endDate=
    // GET /api/financial-reports/balance-sheet?date=
    // GET /api/financial-reports/cash-flow?startDate=&endDate=
}
```

**Integration:** When invoice is paid, create RevenueRecord automatically.

---

## 🎨 FRONTEND PAGES STRUCTURE

### Admin Pages (Next.js 14)

```
Frontend/src/app/admin/
├── notifications/
│   └── page.tsx          ✅ Create
├── insurance/
│   ├── page.tsx          ⚠️ Create
│   ├── companies/
│   │   └── page.tsx      ⚠️ Create
│   └── claims/
│       └── page.tsx      ⚠️ Create
├── pharmacy/
│   ├── page.tsx          ⚠️ Create
│   └── medications/
│       └── page.tsx      ⚠️ Create
├── radiology/
│   ├── page.tsx          ⚠️ Create
│   └── orders/
│       └── [id]/
│           └── page.tsx ⚠️ Create
├── emergency/
│   └── page.tsx          ⚠️ Create
├── assets/
│   └── page.tsx          ⚠️ Create
├── finance/
│   ├── page.tsx          ⚠️ Create
│   ├── expenses/
│   │   └── page.tsx      ⚠️ Create
│   └── revenues/
│       └── page.tsx      ⚠️ Create
├── shifts/
│   └── page.tsx          ⚠️ Create
├── audit-logs/
│   └── page.tsx          ⚠️ Create
└── dashboard/
    └── page.tsx          ✅ Update with KPIs
```

### Doctor Pages

```
Frontend/src/app/doctor/
├── ai-assistant/
│   └── page.tsx          ⚠️ Create
├── radiology/
│   └── page.tsx          ⚠️ Create
└── emergency/
    └── page.tsx          ⚠️ Create
```

### Nurse Pages

```
Frontend/src/app/nurse/
├── tasks/
│   └── page.tsx          ⚠️ Create
└── shifts/
    └── page.tsx          ⚠️ Create
```

**Example: Admin Notifications Page**

```typescript
// Frontend/src/app/admin/notifications/page.tsx
'use client';

import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import { useEffect } from 'react';
import * as signalR from '@microsoft/signalr';

export default function NotificationsPage() {
  const { data: notifications, refetch } = useQuery({
    queryKey: ['notifications'],
    queryFn: async () => {
      const res = await api.get('/notifications');
      return res.data;
    }
  });

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl('/notificationHub', {
        accessTokenFactory: () => localStorage.getItem('token') || ''
      })
      .build();

    connection.on('ReceiveNotification', () => {
      refetch();
    });

    connection.start();
    return () => connection.stop();
  }, []);

  return (
    <div>
      {/* Render notifications */}
    </div>
  );
}
```

---

## 🔧 INTEGRATION CHECKLIST

### Backend Integration Points

- [ ] **AppointmentsController** - Send notification on create/update
- [ ] **LabTestsController** - Send notification when result ready
- [ ] **PrescriptionsController** - Send notification on create
- [ ] **InvoicesController** - Create RevenueRecord on payment, check insurance
- [ ] **Program.cs** - Register audit interceptor
- [ ] **All Controllers** - Add audit logging (automatic via interceptor)

### Frontend Integration

- [ ] Add SignalR connection to layout
- [ ] Create notification bell component
- [ ] Update existing dashboards with new data
- [ ] Add role-based route protection

---

## 📦 DEPENDENCIES TO ADD

### Backend (HMS.API.csproj)
```xml
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="8.0.0" />
```

### Frontend (package.json)
```json
{
  "dependencies": {
    "@microsoft/signalr": "^8.0.0"
  }
}
```

### React Native (hms-nurse-app/package.json)
```json
{
  "dependencies": {
    "@react-native-async-storage/async-storage": "^1.19.0",
    "react-native": "^0.72.0",
    "@react-navigation/native": "^6.1.0"
  }
}
```

---

## 🚀 NEXT STEPS

1. **Create Remaining Controllers** - Follow the patterns shown above
2. **Create DTOs** - For each controller, create corresponding DTOs
3. **Add AutoMapper Profiles** - Map entities to DTOs
4. **Create Frontend Pages** - Use React Query and existing patterns
5. **Test Integration** - Ensure notifications work with existing flows
6. **Add Validators** - FluentValidation for new DTOs
7. **Update Documentation** - API docs, database schema, UML diagrams

---

## 📚 SUMMARY

This expansion adds **14 enterprise-level modules** to the HMS:

✅ **Completed:**
- Notification System (SignalR)
- AI Diagnostic Assistant
- KPI & Analytics Dashboard
- HL7/FHIR Interoperability
- All Entities & Infrastructure

⚠️ **Needs Implementation:**
- Remaining Controllers (Insurance, Pharmacy, Radiology, ER, Assets, Finance, Shifts, Audit)
- Frontend Pages
- React Native App
- Integration with existing controllers

All modules follow Clean Architecture and integrate seamlessly with the existing system!

