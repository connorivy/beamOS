using System.Reflection;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

[FeatureState]
public record class StructuralApiClientState(
    string? ModelId,
    MethodInfo? SelectedMethod,
    ComplexFieldTypeMarker? ParameterValues,
    List<Lazy<ElementReference>>? LazyElementRefs,
    List<ElementReference> ElementRefs,
    FieldInfo? CurrentlySelectedFieldInfo
)
{
    public StructuralApiClientState()
        : this(null, null, null, null, [], null) { }
}
