namespace HMS.Application.DTOs.Pharmacy;

public class PrescriptionFulfillmentDto
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public int MedicationId { get; set; }
    public int QuantityDispensed { get; set; }
    public int DispensedByUserId { get; set; }
    public DateTime DispensedAt { get; set; }
    public string? Notes { get; set; }
}

public class CreatePrescriptionFulfillmentDto
{
    public int PrescriptionId { get; set; }
    public int MedicationId { get; set; }
    public int QuantityDispensed { get; set; }
    public string? Notes { get; set; }
}

