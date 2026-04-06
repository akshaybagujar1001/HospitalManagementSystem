namespace HMS.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendAppointmentConfirmationAsync(string to, string patientName, DateTime appointmentDate, string doctorName);
    Task SendInvoiceEmailAsync(string to, string patientName, string invoiceNumber, decimal amount);
    Task SendLabResultsEmailAsync(string to, string patientName, string testName);
}

