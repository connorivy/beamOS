using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using BeamOs.CodeGen.Apis.StructuralAnalysisApi;
using Microsoft.AspNetCore.Components;
using Microsoft.OpenApi.Readers;
using MudBlazor;

namespace BeamOs.WebApp.Client.Components.Components.Editor;

public partial class StructuralApiClientComponent : ComponentBase
{
    [Inject]
    private IStructuralAnalysisApiAlphaClient StructuralAnalysisApiAlphaClient { get; init; }

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; init; }

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
        methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetParameters().Length == 1)
            .ToArray();
    }

    private void SelectMethod(MethodInfo method)
    {
        selectedMethod = method;
        var parameterType = method.GetParameters().First().ParameterType;
        //PopulateParameterProperties(parameterType);
        this.parameterValues = GetParameterProperties(parameterType);
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

    private Dictionary<string, object> GetParameterProperties(
        Type parameterType,
        string? propPrefix = null
    )
    {
        Dictionary<string, object> parameterProps = [];
        foreach (
            PropertyInfo property in parameterType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            )
        )
        {
            string propName = GetPropertyName(propPrefix, property.Name);
            Type underlyingType =
                Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (SelectionInfoSingleItemComponent2.IsSimpleType(underlyingType))
            {
                parameterProps.Add(propName, "");
            }
            else
            {
                parameterProps.Add(propName, this.GetParameterProperties(underlyingType));
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
        var parameterType = selectedMethod.GetParameters().First().ParameterType;
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

        selectedMethod.Invoke(StructuralAnalysisApiAlphaClient, new[] { parameterInstance });

        // Optionally, navigate back to the method list or show a success message
        selectedMethod = null;
    }

    //private string TurnInputIntoJson() { }

    private void GoBack()
    {
        selectedMethod = null;
    }
}
