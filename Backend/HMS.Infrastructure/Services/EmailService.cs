using HMS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HMS.Infrastructure.Services;

public class EmailService : IEmailService
{
    // In production, integrate with SendGrid, SMTP, or other email service
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        // TODO: Implement actual email sending
        _logger.LogInformation($"Email would be sent to {to} with subject: {subject}");
        await Task.CompletedTask;
    }

    public async Task SendAppointmentConfirmationAsync(string to, string patientName, DateTime appointmentDate, string doctorName)
    {
        var subject = "Appointment Confirmation";
        var body = $@"
            <h2>Appointment Confirmation</h2>
            <p>Dear {patientName},</p>
            <p>Your appointment with Dr. {doctorName} has been confirmed.</p>
            <p><strong>Date:</strong> {appointmentDate:yyyy-MM-dd}</p>
            <p><strong>Time:</strong> {appointmentDate:HH:mm}</p>
            <p>Thank you for choosing our hospital.</p>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendInvoiceEmailAsync(string to, string patientName, string invoiceNumber, decimal amount)
    {
        var subject = "Invoice Generated";
        var body = $@"
            <h2>Invoice Generated</h2>
            <p>Dear {patientName},</p>
            <p>An invoice has been generated for your account.</p>
            <p><strong>Invoice Number:</strong> {invoiceNumber}</p>
            <p><strong>Amount:</strong> ${amount:N2}</p>
            <p>Please find the attached invoice PDF.</p>
        ";
        
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendLabResultsEmailAsync(string to, string patientName, string testName)
    {
        var subject = "Lab Test Results Available";
        var body = $@"
            <h2>Lab Test Results</h2>
            <p>Dear {patientName},</p>
            <p>Your lab test results for {testName} are now available.</p>
            <p>Please log in to your account to view the results.</p>
        ";
        
        await SendEmailAsync(to, subject, body);
    }
}

