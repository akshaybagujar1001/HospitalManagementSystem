using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HMS.Infrastructure.Data;

public class AuditLogInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<AuditLogInterceptor>? _logger;
    private static bool _tableExistsChecked = false;
    private static bool _tableExists = false;

    public AuditLogInterceptor(ILogger<AuditLogInterceptor>? logger = null)
    {
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SaveAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SaveAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SaveAuditLogs(DbContext? context)
    {
        if (context == null || context is not HmsDbContext hmsContext) return;

        // Check if AuditLogs table exists (only check once per application lifetime)
        if (!_tableExistsChecked)
        {
            try
            {
                // Try to query the table to see if it exists
                _ = hmsContext.Database.ExecuteSqlRaw("SELECT TOP 1 1 FROM AuditLogs");
                _tableExists = true;
                _tableExistsChecked = true;
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 208) // Invalid object name
            {
                // Table doesn't exist - skip audit logging
                _tableExists = false;
                _tableExistsChecked = true;
                _logger?.LogWarning("AuditLogs table does not exist. Audit logging is disabled. Please run migrations to create the table.");
            }
            catch
            {
                // Other errors - assume table doesn't exist to be safe
                _tableExists = false;
                _tableExistsChecked = true;
            }
        }

        // Skip audit logging if table doesn't exist
        if (!_tableExists)
        {
            return;
        }

        try
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog && 
                           (e.State == EntityState.Added || 
                            e.State == EntityState.Modified || 
                            e.State == EntityState.Deleted))
                .ToList();

            foreach (var entry in entries)
            {
                var auditLog = new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = GetEntityId(entry.Entity),
                    Action = entry.State.ToString(),
                    OldValues = entry.State == EntityState.Modified || entry.State == EntityState.Deleted
                        ? JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]))
                        : null,
                    NewValues = entry.State == EntityState.Added || entry.State == EntityState.Modified
                        ? JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]))
                        : null,
                    Timestamp = DateTime.UtcNow
                };

                // Try to get UserId from context if available (requires custom implementation)
                // For now, leaving it null - can be set by controllers
                
                hmsContext.AuditLogs.Add(auditLog);
            }
        }
        catch (Exception ex)
        {
            // If audit logging fails, log the error but don't fail the main operation
            _logger?.LogError(ex, "Error creating audit log entry. Continuing without audit log.");
            // Remove any audit logs that were added to prevent transaction failure
            var auditLogEntries = context.ChangeTracker.Entries<AuditLog>().ToList();
            foreach (var auditEntry in auditLogEntries)
            {
                auditEntry.State = EntityState.Detached;
            }
        }
    }

    private int? GetEntityId(object entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        return idProperty?.GetValue(entity) as int?;
    }
}

