namespace HMS.Application.DTOs.Fhir;

public class FhirObservationDto
{
    public string ResourceType { get; set; } = "Observation";
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public FhirCodeableConceptDto Code { get; set; } = new();
    public FhirReferenceDto Subject { get; set; } = new();
    public string EffectiveDateTime { get; set; } = string.Empty;
    public string? ValueString { get; set; }
}

public class FhirCodeableConceptDto
{
    public List<FhirCodingDto> Coding { get; set; } = new();
}

