using System.Reflection;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Client.Components.Components.Editor.PropertyEnumerators;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BeamOs.WebApp.Client.Components.Components.Editor;

public partial class SelectionInfoSingleItemComponent2 : ComponentBase
{
    [Parameter]
    public required string ObjectName { get; init; }

    [Parameter]
    public required object? ObjectToDisplay { get; set; }

    [Parameter]
    public EventCallback<object> ObjectToDisplayChanged { get; set; }

    private Dictionary<Type, IPropertyEnumerator> propertyEnumerators =
        new() { { typeof(Point), new PointPropertyEnumerator() } };

    public static PropertyInfo[] GetPublicInstanceProps(object obj) =>
        obj.GetType()
            .GetProperties(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
            );

    private static readonly HashSet<Type> simpleTypes =
    [
        typeof(bool),
        typeof(int),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(string),
        typeof(Enum),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid)
    ];

    public static bool IsSimpleType(Type t) => simpleTypes.Contains(t);

    private static readonly Converter<object> StringObjectConverter =
        new() { SetFunc = value => value?.ToString(), GetFunc = text => text?.ToString(), };
}
