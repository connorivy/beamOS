namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

public readonly struct SimpleFieldTypeMarker
{
    public required Type FieldType { get; init; }
    public required int FieldNum { get; init; }
    public bool IsRequired { get; init; }
    public object? Value { get; init; }
}
