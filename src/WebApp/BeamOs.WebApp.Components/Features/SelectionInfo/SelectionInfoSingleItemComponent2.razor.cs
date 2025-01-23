//using System.Reflection;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using BeamOs.WebApp.Components.Features.StructuralApi;
//using Fluxor;
//using Fluxor.Blazor.Web.Components;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Web;
//using MudBlazor;

//namespace BeamOs.WebApp.Components.Features.SelectionInfo;

//public partial class SelectionInfoSingleItemComponent2 : FluxorComponent
//{
//    [Parameter]
//    public required string ObjectName { get; init; }

//    [Parameter]
//    public required object? ObjectToDisplay { get; set; }

//    [Parameter]
//    public required string CanvasId { get; init; }

//    [Parameter]
//    public EventCallback<object> ObjectToDisplayChanged { get; set; }

//    [Inject]
//    private IDispatcher Dispatcher { get; init; }

//    protected override void OnInitialized()
//    {
//        base.OnInitialized();
//        //state.Select(s => s.EditorState[this.CanvasId]);
//        //this.SubscribeToAction<MoveNodeCommand>(command =>
//        //{
//        //    if (command.CanvasId == this.CanvasId)
//        //    {
//        //        this.StateHasChanged();
//        //    }
//        //});
//    }

//    //private Dictionary<Type, IPropertyEnumerator> propertyEnumerators =
//    //    new() { { typeof(Point), new PointPropertyEnumerator() } };

//    public static PropertyInfo[]? GetPublicInstanceProps(object? obj) =>
//        GetPublicInstanceProps(obj?.GetType());

//    public static PropertyInfo[]? GetPublicInstanceProps(Type? type) =>
//        type?.GetProperties(
//            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
//        );

//    private MudTextField<bool?> MudTextFieldBoolRef
//    {
//        set =>
//            this.Dispatcher.Dispatch(
//                new FormInputCreated(new(() => value.InputReference.ElementReference))
//            );
//    }

//    private MudTextField<double?> MudTextFieldDoubleRef
//    {
//        set =>
//            this.Dispatcher.Dispatch(
//                new FormInputCreated(new(() => value.InputReference.ElementReference))
//            );
//    }

//    private MudTextField<int?> MudTextFieldIntRef
//    {
//        set =>
//            this.Dispatcher.Dispatch(
//                new FormInputCreated(new(() => value.InputReference.ElementReference))
//            );
//    }

//    private MudTextField<object?> MudTextFieldObjectRef
//    {
//        set =>
//            this.Dispatcher.Dispatch(
//                new FormInputCreated(new(() => value.InputReference.ElementReference))
//            );
//    }

//    private void OnFocus(int fieldNum)
//    {
//        this.Dispatcher.Dispatch(
//            new FieldSelected(new(this.ObjectName, fieldNum, this.ObjectToDisplayChanged))
//        );
//    }

//    // event won't fire is OnlyValidateIfDirty = true and the user hasn't made any changes
//    // https://github.com/MudBlazor/MudBlazor/issues/9732
//    private void OnBlur(FocusEventArgs e)
//    {
//        this.Dispatcher.Dispatch(new CurrentFieldDeselected());
//    }

//    private static readonly HashSet<Type> simpleTypes =
//    [
//        typeof(bool),
//        typeof(int),
//        typeof(float),
//        typeof(double),
//        typeof(decimal),
//        typeof(string),
//        typeof(Enum),
//        typeof(DateTime),
//        typeof(DateTimeOffset),
//        typeof(TimeSpan),
//        typeof(Guid)
//    ];

//    public static bool IsSimpleType(Type t)
//    {
//        if (t.BaseType == typeof(Enum))
//        {
//            return true;
//        }

//        return simpleTypes.Contains(t);
//    }

//    private static readonly Converter<object> StringObjectConverter =
//        new() { SetFunc = value => value?.ToString(), GetFunc = text => text?.ToString(), };
//}

//public readonly record struct FormInputCreated(Lazy<ElementReference> LazyReference);

//public static class FormInputCreatedActionReducer
//{
//    [ReducerMethod]
//    public static StructuralApiClientState ReduceFormInputCreated(
//        StructuralApiClientState state,
//        FormInputCreated action
//    )
//    {
//        return state with
//        {
//            LazyElementRefs = state.LazyElementRefs is null
//                ? [action.LazyReference]
//                : new(state.LazyElementRefs) { action.LazyReference }
//        };
//    }
//}

//public readonly struct CurrentFieldDeselected();

//public readonly record struct FieldSelected(FieldInfo FieldInfo);

//public record FieldInfo(string FieldName, int FieldIndex, EventCallback<object> SetValue);

//[FeatureState]
//public record class StructuralApiClientState(
//    string? ModelId,
//    MethodInfo? SelectedMethod,
//    ComplexFieldTypeMarker? ParameterValues,
//    List<Lazy<ElementReference>>? LazyElementRefs,
//    List<ElementReference> ElementRefs,
//    FieldInfo? CurrentlySelectedFieldInfo
//)
//{
//    public StructuralApiClientState()
//        : this(null, null, null, null, [], null) { }
//}

//public static class FieldSelectedActionReducer
//{
//    [ReducerMethod]
//    public static StructuralApiClientState ReduceFieldSelectedAction(
//        StructuralApiClientState state,
//        FieldSelected action
//    )
//    {
//        return state with { CurrentlySelectedFieldInfo = action.FieldInfo };
//    }
//}

//public static class ElementReferencesEvaluatedActionReducer
//{
//    [ReducerMethod]
//    public static StructuralApiClientState ReduceElementReferencesEvaluated(
//        StructuralApiClientState state,
//        ElementReferencesEvaluated action
//    )
//    {
//        return state with { LazyElementRefs = null, ElementRefs = action.ElementReferences };
//    }
//}

//[JsonConverter(typeof(ComplexFieldTypeMarkerConverter))]
//public class ComplexFieldTypeMarker(bool isRequired) : Dictionary<string, object?>
//{
//    [JsonIgnore]
//    public bool IsRequired { get; } = isRequired;

//    [JsonIgnore]
//    public Dictionary<string, object> ValuesWithDisplayInformation { get; } = [];

//    [JsonIgnore]
//    public int NumRecordsAutoFilled { get; set; }

//    public void Add2(string key, object value)
//    {
//        this.ValuesWithDisplayInformation.Add(key, value);
//        this.Add(key, value is ComplexFieldTypeMarker ? value : null);
//    }

//    public object Get(string key)
//    {
//        if (this.ValuesWithDisplayInformation[key] is SimpleFieldTypeMarker simple)
//        {
//            return simple with { Value = this[key] };
//        }
//        return this.ValuesWithDisplayInformation[key];
//    }

//    public void Set(string key, object value) => this[key] = value;
//}

//public class ComplexFieldTypeMarkerConverter : JsonConverter<ComplexFieldTypeMarker>
//{
//    public override ComplexFieldTypeMarker? Read(
//        ref Utf8JsonReader reader,
//        Type typeToConvert,
//        JsonSerializerOptions options
//    ) => throw new NotImplementedException();

//    public override void Write(
//        Utf8JsonWriter writer,
//        ComplexFieldTypeMarker value,
//        JsonSerializerOptions options
//    )
//    {
//        writer.WriteStartObject();
//        foreach (var kvp in value.ValuesWithDisplayInformation)
//        {
//            if (
//                kvp.Value is SimpleFieldTypeMarker simpleFieldTypeMarker
//                && !simpleFieldTypeMarker.IsRequired
//                && value[kvp.Key] is null
//            )
//            {
//                // Removes optional values that the user hasn't given a value to.
//                // If we don't remove these values, then they could potentially later get
//                // deserialize to a value type from null.
//                continue;
//            }

//            writer.WritePropertyName(kvp.Key);
//            JsonSerializer.Serialize(writer, value[kvp.Key], options);
//        }
//        writer.WriteEndObject();
//    }
//}

//public readonly struct SimpleFieldTypeMarker
//{
//    public required Type FieldType { get; init; }
//    public required int FieldNum { get; init; }
//    public bool IsRequired { get; init; }
//    public object? Value { get; init; }
//}
