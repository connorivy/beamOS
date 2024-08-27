using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BeamOs.CodeGen.Apis.StructuralAnalysisApi;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using Microsoft.AspNetCore.Components;
using Microsoft.OpenApi.Readers;
using MudBlazor;

namespace BeamOs.WebApp.Client.Components.Components.Editor;

public partial class StructuralApiClientComponent : ComponentBase
{
    [Parameter]
    public required string CanvasId { get; init; }

    [Inject]
    private IStructuralAnalysisApiAlphaClient StructuralAnalysisApiAlphaClient { get; init; }

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; init; }

    [Inject]
    private AddEntityContractToEditorCommandHandler AddEntityContractToEditorCommandHandler { get; init; }

    protected override async Task OnInitializedAsync()
    {
        var client = new HttpClient { BaseAddress = new Uri($"https://localhost:7111") };
        var stream = await client.GetStreamAsync("swagger/Alpha%20Release/swagger.json");
        var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

        base.OnInitializedAsync();
    }

    private MethodInfo[] methods;
    private MethodInfo selectedMethod;
    private PropertyInfo[] parameterProperties;
    private object parameterValues;
    private MudForm form;

    protected override void OnInitialized()
    {
        // Replace 'YourClass' with the actual class you want to inspect
        var type = typeof(IStructuralAnalysisApiAlphaClient);
        this.methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetParameters().Length == 1 && !m.Name.StartsWith("Get"))
            .OrderBy(m => m.Name)
            .ToArray();
    }

    private void SelectMethod(MethodInfo method)
    {
        this.selectedMethod = method;
        var parameterType = method.GetParameters().First().ParameterType;
        //PopulateParameterProperties(parameterType);
        this.parameterValues = this.GetParameterProperties(parameterType);
    }

    public readonly struct SimpleFieldTypeMarker
    {
        public SimpleFieldTypeMarker(Type fieldType, bool isRequired = false)
        {
            this.FieldType = fieldType;
            this.IsRequired = isRequired;
        }

        public Type FieldType { get; init; }
        public bool IsRequired { get; init; }
        public object? Value { get; init; } = null;
    }

    public class ComplexFieldTypeMarker(bool isRequired) : Dictionary<string, object?>
    {
        [JsonIgnore]
        public bool IsRequired { get; } = isRequired;

        [JsonIgnore]
        public Dictionary<string, object> ValuesWithDisplayInformation { get; } = [];

        public void Add2(string key, object value)
        {
            this.ValuesWithDisplayInformation.Add(key, value);
            this.Add(key, value is ComplexFieldTypeMarker ? value : null);
        }

        public object Get(string key)
        {
            if (this.ValuesWithDisplayInformation[key] is SimpleFieldTypeMarker simple)
            {
                return simple with { Value = this[key] };
            }
            return this.ValuesWithDisplayInformation[key];
        }

        public void Set(string key, object value) => this[key] = value;
    }

    //private void PopulateParameterProperties(Type parameterType, string? propPrefix = null)
    //{
    //    parameterProperties = parameterType.GetProperties(
    //        BindingFlags.Public | BindingFlags.Instance
    //    );
    //    foreach (PropertyInfo property in parameterProperties)
    //    {
    //        string propName = GetPropertyName(propPrefix, property.Name);
    //        if (SelectionInfoSingleItemComponent2.IsSimpleType(property.PropertyType))
    //        {
    //            this.parameterValues.Add(propName, "");
    //        }
    //        else
    //        {
    //            this.PopulateParameterProperties(property.PropertyType, propName);
    //        }
    //    }
    //}

    private ComplexFieldTypeMarker GetParameterProperties(
        Type parameterType,
        bool? isRequired = null
    )
    {
        ComplexFieldTypeMarker parameterProps = new(isRequired ?? true);
        foreach (
            PropertyInfo property in parameterType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            )
        )
        {
            Type? nullableType = Nullable.GetUnderlyingType(property.PropertyType);
            if (SelectionInfoSingleItemComponent2.IsSimpleType(property.PropertyType))
            {
                parameterProps.Add2(
                    property.Name,
                    new SimpleFieldTypeMarker(property.PropertyType, true)
                );
            }
            else if (
                nullableType is not null
                && SelectionInfoSingleItemComponent2.IsSimpleType(nullableType)
            )
            {
                parameterProps.Add2(property.Name, new SimpleFieldTypeMarker(nullableType));
            }
            else if (nullableType is not null)
            {
                parameterProps.Add2(
                    property.Name,
                    this.GetParameterProperties(nullableType, false)
                );
            }
            else
            {
                parameterProps.Add2(
                    property.Name,
                    this.GetParameterProperties(property.PropertyType, true)
                );
            }
        }
        return parameterProps;
    }

    private static string GetPropertyName(string? prefix, string propName)
    {
        return prefix != null ? $"{prefix}.{propName}" : propName;
    }

    private async Task HandleSubmit()
    {
        var parameterType = this.selectedMethod.GetParameters().First().ParameterType;
        object parameterInstance;

        var serialized = JsonSerializer.Serialize(this.parameterValues);
        try
        {
            parameterInstance = JsonSerializer.Deserialize(serialized, parameterType);
        }
        catch (JsonException ex)
        {
            // Handle JSON deserialization error
            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            return;
        }
        //var parameterType = selectedMethod.GetParameters().First().ParameterType;
        //var parameterInstance = Activator.CreateInstance(parameterType);

        //foreach (var property in parameterProperties)
        //{
        //    var value = Convert.ChangeType(parameterValues[property.Name], property.PropertyType);
        //    property.SetValue(parameterInstance, value);
        //}

        var result = this.selectedMethod.Invoke(
            this.StructuralAnalysisApiAlphaClient,
            new[] { parameterInstance }
        );

        if (result is Task t)
        {
            await t.ConfigureAwait(false);
            result = ((dynamic)t).Result;
        }
        if (result is BeamOsEntityContractBase contract)
        {
            await this.AddEntityContractToEditorCommandHandler.ExecuteAsync(
                new(this.CanvasId, contract)
            );
        }

        // Optionally, navigate back to the method list or show a success message
        this.selectedMethod = null;
    }

    //private string TurnInputIntoJson() { }

    private void GoBack()
    {
        this.selectedMethod = null;
    }

    //public class ComplexFieldTypeMarkerJsonConverter : JsonConverter<ComplexFieldTypeMarker>
    //{
    //    //public override ComplexFieldTypeMarker Read(
    //    //    ref Utf8JsonReader reader,
    //    //    Type typeToConvert,
    //    //    JsonSerializerOptions options
    //    //) =>
    //    //    DateTimeOffset.ParseExact(
    //    //        reader.GetString()!,
    //    //        "MM/dd/yyyy",
    //    //        CultureInfo.InvariantCulture
    //    //    );

    //    public override void Write(
    //        Utf8JsonWriter writer,
    //        DateTimeOffset dateTimeValue,
    //        JsonSerializerOptions options
    //    ) =>
    //        writer.WriteStringValue(
    //            dateTimeValue.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
    //        );

    //    public override void Write(
    //        Utf8JsonWriter writer,
    //        ComplexFieldTypeMarker value,
    //        JsonSerializerOptions options
    //    ) => throw new NotImplementedException();
    //}
}
