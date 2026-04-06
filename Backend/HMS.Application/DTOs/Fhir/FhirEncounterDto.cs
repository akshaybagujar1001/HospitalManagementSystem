namespace HMS.Application.DTOs.Fhir;

public class FhirEncounterDto
{
    public string ResourceType { get; set; } = "Encounter";
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public FhirCodingDto Class { get; set; } = new();
    public FhirReferenceDto Subject { get; set; } = new();
    public FhirPeriodDto Period { get; set; } = new();
}

public class FhirCodingDto
{
    public string Code { get; set; } = string.Empty;
    public string? Display { get; set; }
}

public class FhirReferenceDto
{
    public string Reference { get; set; } = string.Empty;
}

public class FhirPeriodDto
{
    public string Start { get; set; } = string.Empty;
    public string? End { get; set; }
}

