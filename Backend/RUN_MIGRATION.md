# 🔧 URGENT: Run Migration to Create AuditLogs Table

## Problem
The `AuditLogs` table doesn't exist in your database, causing ALL database operations to fail with:
```
Invalid object name 'AuditLogs'
```

## Solution: Run Migration

### Step 1: Stop the Backend API
Press `Ctrl+C` in the terminal where the API is running, or close it.

### Step 2: Run Migration Commands

Open PowerShell and run:

```powershell
cd "C:\Users\Flori\Desktop\Hospital management system\Backend\HMS.API"
dotnet ef migrations add AddAuditLogsTable --project ../HMS.Infrastructure --startup-project .
dotnet ef database update --project ../HMS.Infrastructure --startup-project .
```

### Step 3: Restart Backend
Start the backend again. Now all operations should work!

---

## What This Does

1. **Creates a migration file** that defines the `AuditLogs` table structure
2. **Applies the migration** to your SQL Server database, creating the table
3. **Fixes all database operations** - prescriptions, appointments, insurance companies, etc.

---

## Alternative: If Migration Fails

If you get errors, you can temporarily disable the audit interceptor:

1. Open `Backend/HMS.API/Program.cs`
2. Comment out this line:
   ```csharp
   // options.AddInterceptors(new HMS.Infrastructure.Data.AuditLogInterceptor());
   ```
3. Restart backend
4. **Note**: This disables audit logging. You should still create the table later.

---

## Verify It Worked

After running the migration, check your database:

```sql
USE HMSDB;
SELECT * FROM AuditLogs;
```

If the query runs without error, the table exists! ✅

