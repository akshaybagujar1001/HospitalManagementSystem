namespace HMS.Application.DTOs.Pharmacy;

public class MedicationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? DosageForm { get; set; }
    public string? Strength { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public decimal Price { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Manufacturer { get; set; }
    public bool IsActive { get; set; }
}

public class CreateMedicationDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? DosageForm { get; set; }
    public string? Strength { get; set; }
    public int? StockQuantity { get; set; }
    public int ReorderLevel { get; set; } = 10;
    public decimal Price { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Manufacturer { get; set; }
}

public class UpdateMedicationDto
{
    public string Name { get; set; } = string.Empty;
    public string? DosageForm { get; set; }
    public string? Strength { get; set; }
    public int ReorderLevel { get; set; }
    public decimal Price { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Manufacturer { get; set; }
    public bool IsActive { get; set; }
}

public class AdjustStockDto
{
    public int QuantityChange { get; set; }
    public string? Reason { get; set; }
}

