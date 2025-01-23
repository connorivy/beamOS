//using System.Collections.Generic;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Text.Json;
//using System.Text.Json.Nodes;
//using System.Xml.Linq;
//using BeamOs.CodeGen.StructuralAnalysisApiClient;
//using BeamOs.WebApp.Components.Features.Editor;
//using BeamOs.WebApp.Components.Features.SelectionInfo;
//using BeamOs.WebApp.EditorCommands;
//using Fluxor;
//using Fluxor.Blazor.Web.Components;
//using Microsoft.AspNetCore.Components;
//using Microsoft.JSInterop;
//using MudBlazor;

//namespace BeamOs.WebApp.Components.Features.StructuralApi;

//public partial class StructuralApiComponent : FluxorComponent
//{
//    [Parameter]
//    public required string CanvasId { get; init; }

//    [Parameter]
//    public string? Class { get; init; }

//    [Inject]
//    private IStructuralAnalysisApiClientV1 StructuralAnalysisApiAlphaClient { get; init; }

//    [Inject]
//    private IStateSelection<
//        AllEditorComponentState,
//        EditorComponentState
//    > EditorState { get; init; }

//    //[Inject]
//    //private IHttpClientFactory HttpClientFactory { get; init; }

//    //[Inject]
//    //private AddEntityContractToEditorCommandHandler AddEntityContractToEditorCommandHandler { get; init; }

//    //[Inject]
//    //private GenericCommandHandler GenericCommandHandler { get; init; }

//    [Inject]
//    private IJSRuntime Js { get; init; }

//    [Inject]
//    private IState<StructuralApiClientState> State { get; init; }

//    [Inject]
//    private IDispatcher Dispatcher { get; init; }

//    protected override async Task OnInitializedAsync()
//    {
//        //var client = new HttpClient { BaseAddress = new Uri($"http://localhost:7111") };
//        //var stream = await client.GetStreamAsync("swagger/Alpha%20Release/swagger.json");
//        //var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

//        base.OnInitializedAsync();
//    }

//    private static HashSet<string> methodsToExclude =
//    [
//        nameof(IStructuralAnalysisApiClientV1.CreateModelAsync),
//        //nameof(IStructuralAnalysisApiClientV1.GetElement1dsAsync),
//        nameof(IStructuralAnalysisApiClientV1.GetModelAsync),
//    //nameof(IStructuralAnalysisApiClientV1.GetModelResultsAsync),
//    //nameof(IStructuralAnalysisApiClientV1.GetModelsAsync),
//    //nameof(IStructuralAnalysisApiClientV1.GetMomentDiagramAsync),
//    //nameof(IStructuralAnalysisApiClientV1.GetMomentLoadsAsync),
//    //nameof(IStructuralAnalysisApiClientV1.GetNodeResultsAsync),
//    //nameof(IStructuralAnalysisApiClientV1.GetShearDiagramAsync),
//    //nameof(IStructuralAnalysisApiClientV1.GetSingleElement1dAsync),
//    //nameof(IStructuralAnalysisApiClientV1.GetSingleNodeResultAsync),
//    ];

//    static StructuralApiComponent()
//    {
//        methods = typeof(IStructuralAnalysisApiClientV1)
//            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
//            .Where(
//                m =>
//                    m.GetParameters().LastOrDefault() is var x
//                    && x is not null
//                    && x.ParameterType != typeof(CancellationToken)
//                    && !methodsToExclude.Contains(m.Name)
//            )
//            .OrderBy(m => m.Name)
//            .ToArray();
//    }

//    private static readonly MethodInfo[] methods;

//    protected override void OnInitialized()
//    {
//        this.EditorState.Select(s => s.EditorState[this.CanvasId]);
//        this.SubscribeToAction<ChangeSelectionCommand>(async c =>
//        {
//            if (c.SelectedObjects.Length != 1)
//            {
//                return;
//            }

//            var state = this.State.Value;
//            string objectIdName = $"{c.SelectedObjects[0].TypeName}Id";
//            if (
//                state.CurrentlySelectedFieldInfo is not null
//                && state
//                    .CurrentlySelectedFieldInfo
//                    .FieldName
//                    .EndsWith(objectIdName, StringComparison.OrdinalIgnoreCase)
//            )
//            {
//                await state
//                    .CurrentlySelectedFieldInfo
//                    .SetValue
//                    .InvokeAsync(c.SelectedObjects[0].Id);

//                if (state.ElementRefs.Count >= state.CurrentlySelectedFieldInfo.FieldIndex + 2)
//                {
//                    await this.State
//                        .Value
//                        .ElementRefs[state.CurrentlySelectedFieldInfo.FieldIndex + 1]
//                        .FocusAsync();
//                }
//            }
//        });

//        base.OnInitialized();
//    }

//    protected override async Task OnAfterRenderAsync(bool firstRender)
//    {
//        if (
//            this.State.Value.LazyElementRefs is not null
//            && this.State.Value.LazyElementRefs.Count > 0
//        )
//        {
//            this.Dispatcher.Dispatch(
//                new ElementReferencesEvaluated(
//                    this.State.Value.LazyElementRefs.Select(x => x.Value).ToList()
//                )
//            );

//            if (
//                this.State.Value.ParameterValues?.NumRecordsAutoFilled is int numRecordsFilled
//                && this.State.Value.ElementRefs.Count > numRecordsFilled
//            )
//            {
//                await this.State
//                    .Value
//                    .ElementRefs[this.State.Value.ParameterValues.NumRecordsAutoFilled]
//                    .FocusAsync();
//            }
//        }
//        await base.OnAfterRenderAsync(firstRender);
//    }

//    private void SelectMethod(MethodInfo method)
//    {
//        this.Dispatcher.Dispatch(
//            new ApiClientMethodSelected(this.EditorState.Value.LoadedModelId.Value, method)
//        );
//    }

//    private async Task HandleSubmit()
//    {
//        var parameterType = this.State.Value.SelectedMethod.GetParameters().First().ParameterType;
//        object parameterInstance;

//        var serialized = JsonSerializer.Serialize(this.State.Value.ParameterValues);
//        try
//        {
//            parameterInstance = JsonSerializer.Deserialize(serialized, parameterType);
//        }
//        catch (JsonException ex)
//        {
//            // Handle JSON deserialization error
//            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
//            return;
//        }

//        var result = this.State
//            .Value
//            .SelectedMethod
//            .Invoke(this.StructuralAnalysisApiAlphaClient, new[] { parameterInstance });

//        if (result is Task t)
//        {
//            await t.ConfigureAwait(false);
//            result = ((dynamic)t).Result;
//        }
//        //if (result is BeamOsEntityContractBase contract)
//        //{
//        //    await this.AddEntityContractToEditorCommandHandler.ExecuteAsync(
//        //        new(this.CanvasId, contract)
//        //    );
//        //}
//        //else if (result is IEnumerable<BeamOsEntityContractBase> entities && entities.Any())
//        //{
//        //    IClientCommand command = entities.First() switch
//        //    {
//        //        ShearDiagramResponse
//        //            => new AddEntitiesToEditorCommand<ShearDiagramResponse>(
//        //                this.CanvasId,
//        //                entities.Cast<ShearDiagramResponse>()
//        //            ),
//        //        MomentDiagramResponse
//        //            => new AddEntitiesToEditorCommand<MomentDiagramResponse>(
//        //                this.CanvasId,
//        //                entities.Cast<MomentDiagramResponse>()
//        //            ),
//        //        _ => throw new NotImplementedException(),
//        //    };
//        //    await this.GenericCommandHandler.ExecuteAsync(command);
//        //}

//        this.GoBack();
//    }

//    private static string CleanMethodName(string name)
//    {
//        if (name.EndsWith("Async", StringComparison.Ordinal))
//        {
//            return name[..^5];
//        }
//        return name;
//    }

//    private static (Color, string) GetChipInfoForMethod(MethodInfo methodInfo)
//    {
//        if (
//            methodInfo.Name.StartsWith("Create", StringComparison.Ordinal)
//            || methodInfo.Name.StartsWith("Run", StringComparison.Ordinal)
//        )
//        {
//            return (Color.Success, "POST");
//        }
//        else if (methodInfo.Name.StartsWith("Delete", StringComparison.Ordinal))
//        {
//            return (Color.Error, "DEL");
//        }
//        else if (
//            methodInfo.Name.StartsWith("Patch", StringComparison.Ordinal)
//            || methodInfo.Name.StartsWith("Update", StringComparison.Ordinal)
//        )
//        {
//            return (Color.Warning, "PATCH");
//        }
//        else if (methodInfo.Name.StartsWith("Get", StringComparison.Ordinal))
//        {
//            return (Color.Info, "GET");
//        }
//        throw new Exception("todo");
//    }

//    private void GoBack()
//    {
//        this.Dispatcher.Dispatch(
//            new ApiClientMethodSelected(this.EditorState.Value.LoadedModelId.Value, null)
//        );
//    }

//    public static ComplexFieldTypeMarker GetSettableParameterProperties(
//        Type parameterType,
//        string modelId,
//        bool isObjectRequired
//    )
//    {
//        int numSelectableFieldsCreated = 0;
//        return GetSettableParameterProperties(
//            parameterType,
//            modelId,
//            isObjectRequired,
//            ref numSelectableFieldsCreated
//        );
//    }

//    private static ComplexFieldTypeMarker GetSettableParameterProperties(
//        Type parameterType,
//        string modelId,
//        bool isObjectRequired,
//        ref int numSelectableFieldsCreated
//    )
//    {
//        int numItemsFilledIn = 0;
//        ComplexFieldTypeMarker parameterProps = new(isObjectRequired);
//        foreach (
//            PropertyInfo property in parameterType
//                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                .Where(p => p.GetSetMethod() is not null)
//        )
//        {
//            var isPropRequired =
//                property.GetCustomAttribute<System.Runtime.CompilerServices.RequiredMemberAttribute>()
//                    is not null;
//            Type propertyType =
//                Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
//            if (SelectionInfoSingleItemComponent2.IsSimpleType(propertyType))
//            {
//                parameterProps.Add2(
//                    property.Name,
//                    new SimpleFieldTypeMarker()
//                    {
//                        FieldType = propertyType,
//                        FieldNum = numSelectableFieldsCreated++,
//                        IsRequired = isObjectRequired && isPropRequired
//                    }
//                );
//                if (property.Name == "ModelId")
//                {
//                    parameterProps.Set(property.Name, modelId);
//                    numItemsFilledIn++;
//                }
//            }
//            else
//            {
//                parameterProps.Add2(
//                    property.Name,
//                    GetSettableParameterProperties(
//                        propertyType,
//                        modelId,
//                        isObjectRequired && isPropRequired,
//                        ref numSelectableFieldsCreated
//                    )
//                );
//            }
//        }
//        parameterProps.NumRecordsAutoFilled = numItemsFilledIn;
//        return parameterProps;
//    }

//    public static ComplexFieldTypeMarker GetSettableParameterProperties(
//        IEnumerable<(bool IsRequired, Type MemberType, string MemberName)> parameterTypes,
//        string modelId,
//        bool isObjectRequired,
//        ref int numSelectableFieldsCreated
//    )
//    {
//        int numItemsFilledIn = 0;
//        ComplexFieldTypeMarker parameterProps = new(isObjectRequired);

//        foreach (var item in parameterTypes)
//        {
//            var allSubItems = item.MemberType
//                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                .Where(p => p.GetSetMethod() is not null)
//                .Select(
//                    p =>
//                        (
//                            p.GetCustomAttribute<RequiredMemberAttribute>() is not null,
//                            Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType,
//                            p.Name
//                        )
//                );
//            if (SelectionInfoSingleItemComponent2.IsSimpleType(item.MemberType))
//            {
//                allSubItems = allSubItems.Prepend(item);
//            }

//            foreach (var (isRequired, propType, namelower) in allSubItems)
//            {
//                var name = char.ToUpperInvariant(namelower[0]) + namelower[1..];
//                if (SelectionInfoSingleItemComponent2.IsSimpleType(propType))
//                {
//                    parameterProps.Add2(
//                        name,
//                        new SimpleFieldTypeMarker()
//                        {
//                            FieldType = propType,
//                            FieldNum = numSelectableFieldsCreated++,
//                            IsRequired = isObjectRequired && isRequired
//                        }
//                    );
//                    if (name == "ModelId")
//                    {
//                        parameterProps.Set(name, modelId);
//                        numItemsFilledIn++;
//                    }
//                }
//                else
//                {
//                    parameterProps.Add2(
//                        name,
//                        GetSettableParameterProperties(
//                            propType,
//                            modelId,
//                            isObjectRequired && isRequired,
//                            ref numSelectableFieldsCreated
//                        )
//                    );
//                }
//            }
//        }
//        parameterProps.NumRecordsAutoFilled = numItemsFilledIn;
//        return parameterProps;
//    }
//}

//public readonly record struct ElementReferencesEvaluated(List<ElementReference> ElementReferences);

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

//public readonly record struct ApiClientMethodSelected(Guid ModelId, MethodInfo? MethodInfo);

//public static class ApiClientMethodSelectedActionReducer
//{
//    [ReducerMethod]
//    public static StructuralApiClientState ReduceApiClientMethodSelectedAction(
//        StructuralApiClientState state,
//        ApiClientMethodSelected action
//    )
//    {
//        if (action.MethodInfo is null)
//        {
//            return state with
//            {
//                SelectedMethod = null,
//                ParameterValues = null,
//                CurrentlySelectedFieldInfo = null,
//                LazyElementRefs = null,
//                ElementRefs =  []
//            };
//        }

//        int fieldNum = 0;
//        return state with
//        {
//            SelectedMethod = action.MethodInfo,
//            ParameterValues = StructuralApiComponent.GetSettableParameterProperties(
//                action
//                    .MethodInfo
//                    .GetParameters()
//                    .Select(
//                        p =>
//                            (
//                                true,
//                                Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType,
//                                p.Name
//                            )
//                    ),
//                action.ModelId.ToString(),
//                true,
//                ref fieldNum
//            )
//        };
//    }
//}
