using System.Reflection;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Client.Components.Components.Editor.PropertyEnumerators;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Components.Editor;

public partial class SelectionInfoSingleItemComponent2 : ComponentBase
{
    [Parameter]
    public required string ObjectName { get; init; }

    [Parameter]
    public required object ObjectToDisplay { get; init; }

    private Type ObjectType { get; set; }

    public PropertyInfo[] propertyInfos { get; set; }

    private Dictionary<Type, IPropertyEnumerator> propertyEnumerators =
        new() { { typeof(Point), new PointPropertyEnumerator() } };

    protected override void OnParametersSet()
    {
        this.ObjectType = this.ObjectToDisplay.GetType();
        this.propertyInfos = this.ObjectToDisplay
            .GetType()
            .GetProperties(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
            );

        base.OnParametersSet();
    }

    public PropertyInfo[] GetPublicInstanceProps(object obj) =>
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
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid)
    ];

    public bool IsSimpleType(Type t) => simpleTypes.Contains(t);
}
