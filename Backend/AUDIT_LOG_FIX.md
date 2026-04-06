# AuditLog Table Fix - Complete Solution

## 1. Why This Error Happens

When you use an `AuditLogInterceptor` in Entity Framework Core, the interceptor runs **during** `SaveChanges()`. Here's what happens:

1. You call `SaveChangesAsync()` to insert an `InsuranceCompany`
2. The `AuditLogInterceptor.SavingChangesAsync()` method is triggered
3. The interceptor creates `AuditLog` entities and adds them to the context using `hmsContext.AuditLogs.Add(auditLog)`
4. EF Core tries to save **both** the `InsuranceCompany` AND the `AuditLog` in the same transaction
5. The SQL command tries to insert into `AuditLogs` table first
6. **The table doesn't exist**, so the entire transaction fails

**Key Point**: The interceptor adds audit logs **before** the actual save, so EF Core tries to insert both entities in one transaction. If the `AuditLogs` table doesn't exist, the whole operation fails.

---

## 2. Complete AuditLog Entity (Already Exists)

The `AuditLog` entity is already defined in `HMS.Domain/Entities/AuditLog.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    
    public int? UserId { get; set; } // Nullable for system actions
    
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    
    [Required]
    [MaxLength(100)]
    public string EntityName { get; set; } = string.Empty; // Patient, Appointment, etc.
    
    public int? EntityId { get; set; }
    
    public string? OldValues { get; set; } // JSON string
    
    public string? NewValues { get; set; } // JSON string
    
    [MaxLength(50)]
    public string? IPAddress { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
```

---

## 3. DbSet in HmsDbContext (Already Exists)

The `DbSet<AuditLog>` is already in `HmsDbContext.cs`:

```csharp
public DbSet<AuditLog> AuditLogs { get; set; }
```

---

## 4. Complete OnModelCreating Configuration

The configuration has been updated in `HmsDbContext.cs`:

```csharp
// AuditLog configuration
modelBuilder.Entity<AuditLog>(entity =>
{
    entity.ToTable("AuditLogs");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).ValueGeneratedOnAdd();
    entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
    entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
    entity.Property(e => e.IPAddress).HasMaxLength(50);
    entity.Property(e => e.Description).HasMaxLength(500);
    entity.Property(e => e.Timestamp).IsRequired();
    
    // Foreign key to User (optional)
    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.SetNull);
    
    // Indexes for performance
    entity.HasIndex(e => new { e.EntityName, e.EntityId });
    entity.HasIndex(e => e.Timestamp);
    entity.HasIndex(e => e.UserId);
});
```

**Key Configuration Points:**
- `ToTable("AuditLogs")` - Explicitly sets the table name
- `HasKey(e => e.Id)` - Defines primary key
- `ValueGeneratedOnAdd()` - Auto-increment ID
- Property constraints match the entity attributes
- Foreign key to `User` with `SetNull` on delete (since UserId is nullable)
- Indexes for common query patterns

---

## 5. EF Core Migration Commands

Run these commands in PowerShell from the `Backend/HMS.API` directory:

```powershell
# Navigate to API project
cd "C:\Users\Flori\Desktop\Hospital management system\Backend\HMS.API"

# Create migration for AuditLogs table
dotnet ef migrations add AddAuditLogsTable --project ../HMS.Infrastructure --startup-project .

# Apply migration to database
dotnet ef database update --project ../HMS.Infrastructure --startup-project .
```

**Alternative**: If you want to create a migration that includes both `AuditLogs` and `RevenueRecords` and `InsuranceCompanies`:

```powershell
dotnet ef migrations add AddMissingTables --project ../HMS.Infrastructure --startup-project .
dotnet ef database update --project ../HMS.Infrastructure --startup-project .
```

---

## 6. Temporarily Disable Audit Interceptor (For Testing)

### Option A: Comment Out in Program.cs

In `Backend/HMS.API/Program.cs`, temporarily comment out the interceptor:

```csharp
// Database Configuration
builder.Services.AddDbContext<HmsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    // Temporarily disabled for testing
    // options.AddInterceptors(new HMS.Infrastructure.Data.AuditLogInterceptor());
});
```

### Option B: Conditional Registration

Modify `Program.cs` to conditionally register the interceptor:

```csharp
// Database Configuration
builder.Services.AddDbContext<HmsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    
    // Only add interceptor if AuditLogs table exists (check via configuration)
    var enableAuditLogging = builder.Configuration.GetValue<bool>("EnableAuditLogging", true);
    if (enableAuditLogging)
    {
        options.AddInterceptors(new HMS.Infrastructure.Data.AuditLogInterceptor());
    }
});
```

Then add to `appsettings.Development.json`:

```json
{
  "EnableAuditLogging": false
}
```

### Option C: Skip Audit Logging for Specific Entities

Modify `AuditLogInterceptor.cs` to skip certain entities:

```csharp
private void SaveAuditLogs(DbContext? context)
{
    if (context == null || context is not HmsDbContext hmsContext) return;

    var entries = context.ChangeTracker.Entries()
        .Where(e => e.Entity is not AuditLog && 
                   // Skip InsuranceCompany for testing
                   e.Entity is not InsuranceCompany &&
                   (e.State == EntityState.Added || 
                    e.State == EntityState.Modified || 
                    e.State == EntityState.Deleted))
        .ToList();
    
    // ... rest of the method
}
```

---

## 7. Verify the Fix

After running the migration, verify the table exists:

```sql
-- Run in SQL Server Management Studio or Azure Data Studio
USE HMSDB;
GO

SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AuditLogs';
GO

SELECT * FROM AuditLogs;
GO
```

You should see:
- Table `AuditLogs` exists
- Table has columns: `Id`, `UserId`, `Action`, `EntityName`, `EntityId`, `OldValues`, `NewValues`, `IPAddress`, `Description`, `Timestamp`
- Foreign key constraint to `Users` table

---

## Summary

1. ✅ **AuditLog entity** - Already exists
2. ✅ **DbSet<AuditLog>** - Already exists in HmsDbContext
3. ✅ **OnModelCreating configuration** - Updated with complete configuration
4. ✅ **Migration commands** - Provided above
5. ✅ **Temporary disable options** - Three methods provided

**Next Step**: Run the migration commands to create the `AuditLogs` table, then try creating an `InsuranceCompany` again.

