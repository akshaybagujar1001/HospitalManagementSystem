namespace HMS.Application.DTOs.Fhir;

public class FhirPatientDto
{
    public string ResourceType { get; set; } = "Patient";
    public string Id { get; set; } = string.Empty;
    public List<FhirIdentifierDto> Identifier { get; set; } = new();
    public List<FhirHumanNameDto> Name { get; set; } = new();
    public string? Gender { get; set; }
    public string? BirthDate { get; set; }
    public List<FhirContactPointDto> Telecom { get; set; } = new();
}

public class FhirIdentifierDto
{
    public string System { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class FhirHumanNameDto
{
    public string Family { get; set; } = string.Empty;
    public List<string> Given { get; set; } = new();
}

public class FhirContactPointDto
{
    public string System { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

