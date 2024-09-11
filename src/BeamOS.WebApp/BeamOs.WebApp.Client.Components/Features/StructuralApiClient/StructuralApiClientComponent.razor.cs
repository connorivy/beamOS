using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using BeamOs.CodeGen.Apis.StructuralAnalysisApi;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

public partial class StructuralApiClientComponent : FluxorComponent
{
    [Parameter]
    public required string CanvasId { get; init; }

    [Parameter]
    public required string ModelId { get; init; }

    [Inject]
    private IStructuralAnalysisApiAlphaClient StructuralAnalysisApiAlphaClient { get; init; }

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; init; }

    [Inject]
    private AddEntityContractToEditorCommandHandler AddEntityContractToEditorCommandHandler { get; init; }

    [Inject]
    private IJSRuntime Js { get; init; }

    [Inject]
    private IState<StructuralApiClientState> State { get; init; }

    [Inject]
    private IDispatcher Dispatcher { get; init; }

    protected override async Task OnInitializedAsync()
    {
        //var client = new HttpClient { BaseAddress = new Uri($"http://localhost:7111") };
        //var stream = await client.GetStreamAsync("swagger/Alpha%20Release/swagger.json");
        //var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

        base.OnInitializedAsync();
    }

    static StructuralApiClientComponent()
    {
        methods = typeof(IStructuralAnalysisApiAlphaClient)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetParameters().Length == 1 && !m.Name.StartsWith("Get"))
            .OrderBy(m => m.Name)
            .ToArray();
    }

    private static MethodInfo[] methods;

    protected override void OnInitialized()
    {
        this.SubscribeToAction<ChangeSelectionCommand>(async c =>
        {
            if (c.SelectedObjects.Length != 1)
            {
                return;
            }

            var state = this.State.Value;
            string objectIdName = $"{c.SelectedObjects[0].TypeName}Id";
            if (
                state.CurrentlySelectedFieldInfo is not null
                && state
                    .CurrentlySelectedFieldInfo
                    .FieldName
                    .EndsWith(objectIdName, StringComparison.OrdinalIgnoreCase)
            )
            {
                await state
                    .CurrentlySelectedFieldInfo
                    .SetValue
                    .InvokeAsync(c.SelectedObjects[0].Id);

                await this.State
                    .Value
                    .ElementRefs[state.CurrentlySelectedFieldInfo.FieldIndex + 1]
                    .FocusAsync();
            }
        });
        this.SubscribeToAction<CanvasClicked>(async _ =>
        {
            var state = this.State.Value;
            if (state.CurrentlySelectedFieldInfo is not null)
            {
                await this.State
                    .Value
                    .ElementRefs[state.CurrentlySelectedFieldInfo.FieldIndex]
                    .FocusAsync();
            }
        });

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (
            this.State.Value.LazyElementRefs is not null
            && this.State.Value.LazyElementRefs.Count > 0
        )
        {
            this.Dispatcher.Dispatch(
                new ElementReferencesEvaluated(
                    this.State.Value.LazyElementRefs.Select(x => x.Value).ToList()
                )
            );
            await this.State
                .Value
                .ElementRefs[this.State.Value.ParameterValues.NumRecordsAutoFilled]
                .FocusAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void SelectMethod(MethodInfo method)
    {
        this.Dispatcher.Dispatch(new ApiClientMethodSelected(method));
    }

    private async Task HandleSubmit()
    {
        var parameterType = this.State.Value.SelectedMethod.GetParameters().First().ParameterType;
        object parameterInstance;

        var serialized = JsonSerializer.Serialize(this.State.Value.ParameterValues);
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

        var result = this.State
            .Value
            .SelectedMethod
            .Invoke(this.StructuralAnalysisApiAlphaClient, new[] { parameterInstance });

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
        this.GoBack();
    }

    private static string CleanMethodName(string name)
    {
        if (name.EndsWith("Async", StringComparison.Ordinal))
        {
            return name[..^5];
        }
        return name;
    }

    private static (Color, string) GetChipInfoForMethod(MethodInfo methodInfo)
    {
        if (
            methodInfo.Name.StartsWith("Create", StringComparison.Ordinal)
            || methodInfo.Name.StartsWith("Run", StringComparison.Ordinal)
        )
        {
            return (Color.Success, "POST");
        }
        else if (methodInfo.Name.StartsWith("Delete", StringComparison.Ordinal))
        {
            return (Color.Error, "DEL");
        }
        else if (methodInfo.Name.StartsWith("Patch", StringComparison.Ordinal))
        {
            return (Color.Warning, "PATCH");
        }
        throw new Exception("todo");
    }

    private void GoBack()
    {
        this.Dispatcher.Dispatch(new ApiClientMethodSelected(null));
    }

    public static ComplexFieldTypeMarker GetParameterProperties(
        Type parameterType,
        string modelId,
        bool isObjectRequired,
        int numSelectableFieldsCreated = 0
    )
    {
        int numItemsFilledIn = 0;
        ComplexFieldTypeMarker parameterProps = new(isObjectRequired);
        foreach (
            PropertyInfo property in parameterType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            )
        )
        {
            var isPropRequired = property.GetCustomAttribute<RequiredMemberAttribute>() is not null;
            Type propertyType =
                Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (SelectionInfoSingleItemComponent2.IsSimpleType(propertyType))
            {
                parameterProps.Add2(
                    property.Name,
                    new SimpleFieldTypeMarker()
                    {
                        FieldType = propertyType,
                        FieldNum = numSelectableFieldsCreated++,
                        IsRequired = isObjectRequired && isPropRequired
                    }
                );
                if (property.Name == "ModelId")
                {
                    parameterProps.Set(property.Name, modelId);
                    numItemsFilledIn++;
                }
            }
            else
            {
                parameterProps.Add2(
                    property.Name,
                    GetParameterProperties(
                        propertyType,
                        modelId,
                        isObjectRequired && isPropRequired,
                        numSelectableFieldsCreated
                    )
                );
            }
        }
        parameterProps.NumRecordsAutoFilled = numItemsFilledIn;
        return parameterProps;
    }
}
