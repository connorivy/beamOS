using System.Reflection;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Client.Components.Components.Editor.PropertyEnumerators;
using BeamOs.WebApp.Client.Components.Features.StructuralApiClient;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

    [Inject]
    private IDispatcher Dispatcher { get; init; }

    private Dictionary<Type, IPropertyEnumerator> propertyEnumerators =
        new() { { typeof(Point), new PointPropertyEnumerator() } };

    public static PropertyInfo[] GetPublicInstanceProps(object obj) =>
        obj.GetType()
            .GetProperties(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
            );

    private MudTextField<bool?> MudTextFieldBoolRef
    {
        set =>
            this.Dispatcher.Dispatch(
                new FormInputCreated(new(() => value.InputReference.ElementReference))
            );
    }

    private MudTextField<double?> MudTextFieldDoubleRef
    {
        set =>
            this.Dispatcher.Dispatch(
                new FormInputCreated(new(() => value.InputReference.ElementReference))
            );
    }

    private MudTextField<int?> MudTextFieldIntRef
    {
        set =>
            this.Dispatcher.Dispatch(
                new FormInputCreated(new(() => value.InputReference.ElementReference))
            );
    }

    private MudTextField<object?> MudTextFieldStringRef
    {
        set =>
            this.Dispatcher.Dispatch(
                new FormInputCreated(new(() => value.InputReference.ElementReference))
            );
    }

    private void OnFocus(int fieldNum)
    {
        this.Dispatcher.Dispatch(
            new FieldSelected(new(this.ObjectName, fieldNum, this.ObjectToDisplayChanged))
        );
    }

    // event won't fire is OnlyValidateIfDirty = true and the user hasn't made any changes
    // https://github.com/MudBlazor/MudBlazor/issues/9732
    private void OnBlur(FocusEventArgs e)
    {
        this.Dispatcher.Dispatch(new CurrentFieldDeselected());
    }

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

    public static bool IsSimpleType(Type t)
    {
        if (t.BaseType == typeof(Enum))
        {
            return true;
        }

        return simpleTypes.Contains(t);
    }

    private static readonly Converter<object> StringObjectConverter =
        new() { SetFunc = value => value?.ToString(), GetFunc = text => text?.ToString(), };
}
