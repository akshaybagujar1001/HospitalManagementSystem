namespace HMS.Application.DTOs.Insurance;

public class InsuranceCompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PolicyRules { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInsuranceCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PolicyRules { get; set; }
}

public class UpdateInsuranceCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PolicyRules { get; set; }
    public bool IsActive { get; set; }
}

