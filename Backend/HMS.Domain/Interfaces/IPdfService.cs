namespace HMS.Domain.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateInvoicePdfAsync(int invoiceId);
    Task<byte[]> GeneratePrescriptionPdfAsync(int prescriptionId);
    Task<byte[]> GenerateMedicalReportPdfAsync(int medicalRecordId);
}

