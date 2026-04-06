using System.Text;
using System.Linq;
using FluentValidation;
using HMS.Application.Mappings;
using HMS.Application.Services;
using HMS.API.Hubs;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;
using HMS.Infrastructure.Repositories;
using HMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references in JSON serialization
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 32;
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hospital Management System API",
        Version = "v1",
        Description = "Enterprise-level Hospital Management System API"
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Configuration
builder.Services.AddDbContext<HmsDbContext>((serviceProvider, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    // Enable sensitive data logging for detailed error diagnostics
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    // Add audit logging interceptor with logger
    var logger = serviceProvider.GetService<ILogger<HMS.Infrastructure.Data.AuditLogInterceptor>>();
    options.AddInterceptors(new HMS.Infrastructure.Data.AuditLogInterceptor(logger));
});

// Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfService, PdfService>();

// SignalR
builder.Services.AddSignalR();

// New Enterprise Services  
builder.Services.AddScoped<HMS.Application.Services.INotificationService>(sp =>
{
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    var hubContext = sp.GetRequiredService<IHubContext<NotificationHub>>();
    var logger = sp.GetRequiredService<ILogger<HMS.Application.Services.NotificationService>>();
    // Use dynamic to avoid type issues - the hub context will work the same way
    return new HMS.Application.Services.NotificationService(unitOfWork, hubContext, logger);
});
builder.Services.AddScoped<HMS.Application.Services.IAIDiagnosisService, HMS.Application.Services.AIDiagnosisService>();
builder.Services.AddScoped<HMS.Application.Services.IManagementDashboardService>(sp =>
{
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    var context = sp.GetRequiredService<HmsDbContext>();
    return new HMS.Application.Services.ManagementDashboardService(unitOfWork, context);
});
builder.Services.AddScoped<HMS.Application.Services.IFhirExportService>(sp =>
{
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    var context = sp.GetRequiredService<HmsDbContext>();
    return new HMS.Application.Services.FhirExportService(unitOfWork, context);
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "HMS",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "HMS",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // frontend
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Enable detailed exception page
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

// CORS must be before other middleware
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// SignalR Hub
app.MapHub<NotificationHub>("/api/notificationHub");


// Apply migrations and seed data
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<HmsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Checking for pending migrations...");
        
        // Get pending migrations
        var pendingMigrations = context.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Found {Count} pending migration(s): {Migrations}", 
                pendingMigrations.Count, string.Join(", ", pendingMigrations));
            
            // Check if migration is already in history (might have been marked manually)
            var appliedMigrations = context.Database.GetAppliedMigrations().ToList();
            var migrationsToApply = pendingMigrations.Where(m => !appliedMigrations.Contains(m)).ToList();
            
            if (migrationsToApply.Any())
            {
                logger.LogInformation("Applying {Count} migration(s): {Migrations}", 
                    migrationsToApply.Count, string.Join(", ", migrationsToApply));
                
                try
                {
                    context.Database.Migrate();
                    logger.LogInformation("✅ Migrations applied successfully!");
                }
                catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 2714) // Object already exists
                {
                    // Some tables already exist (created manually or with EnsureCreated)
                    // This is OK - we'll mark the migration as applied manually
                    logger.LogWarning("⚠️ Migration failed because some tables already exist (Error 2714).");
                    logger.LogWarning("This usually happens when tables were created with EnsureCreated() or manually.");
                    logger.LogWarning("Attempting to mark migration as applied...");
                    
                    try
                    {
                        // Mark all pending migrations as applied
                        var productVersion = "8.0.0"; // EF Core version
                        
                        foreach (var migrationName in migrationsToApply)
                        {
                            // Use ExecuteSqlInterpolated for parameterized queries
                            context.Database.ExecuteSqlInterpolated(
                                $@"IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = {migrationName}) 
                                   INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ({migrationName}, {productVersion})");
                            
                            logger.LogInformation("✅ Migration '{MigrationName}' marked as applied.", migrationName);
                        }
                        
                        logger.LogInformation("✅ All migrations marked as applied. Database should be in sync now.");
                    }
                    catch (Exception ex2)
                    {
                        logger.LogError(ex2, "Failed to mark migration as applied. You may need to manually fix the migration history.");
                        logger.LogWarning("⚠️ Continuing anyway - application will start but some features may not work correctly.");
                        logger.LogWarning("💡 To fix manually, run: INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('{MigrationName}', '8.0.0')", 
                            migrationsToApply.First());
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Failed to apply migrations. Error: {Error}", ex.Message);
                    logger.LogWarning("⚠️ Continuing anyway - application will start but database may be out of sync.");
                }
            }
            else
            {
                logger.LogInformation("✅ All pending migrations are already marked as applied in history.");
            }
        }
        else
        {
            logger.LogInformation("No pending migrations. Database is up to date.");
        }
        
        // Verify AuditLogs table exists
        try
        {
            var auditLogsExist = context.Database.ExecuteSqlRaw("SELECT TOP 1 1 FROM AuditLogs");
            logger.LogInformation("AuditLogs table exists. Audit logging is enabled.");
        }
        catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 208)
        {
            logger.LogWarning("⚠️ AuditLogs table does not exist! Audit logging is disabled.");
            logger.LogWarning("To enable audit logging, run: dotnet ef database update --project ../HMS.Infrastructure --startup-project .");
        }
        
        logger.LogInformation("Seeding initial data...");
        SeedData.Seed(context);
        logger.LogInformation("Database setup completed successfully!");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while setting up the database: {Error}", ex.Message);
    logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
    // Continue anyway - don't stop the application
}

app.Run();

