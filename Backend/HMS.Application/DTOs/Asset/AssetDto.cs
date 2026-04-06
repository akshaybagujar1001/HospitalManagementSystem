namespace HMS.Application.DTOs.Asset;

public class AssetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Location { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}

public class CreateAssetDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; }
    public decimal? PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAssetDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Location { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string? Notes { get; set; }
}

public class CreateAssetMaintenanceDto
{
    public int AssetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? MaintenanceType { get; set; }
    public decimal? Cost { get; set; }
    public DateTime? PerformedAt { get; set; }
    public string? Vendor { get; set; }
    public string? Notes { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}

