using System.Reflection;
using System.Text.Json;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.StructuralApi;

public partial class StructuralApiComponent : FluxorComponent
{
    [Parameter]
    public required string CanvasId { get; init; }

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public string? Class { get; init; }

    [Inject]
    private IStructuralAnalysisApiClientV1 StructuralAnalysisApiAlphaClient { get; init; }

    [Inject]
    private IJSRuntime Js { get; init; }

    [Inject]
    private IState<StructuralApiClientState> State { get; init; }

    [Inject]
    private ISnackbar Snackbar { get; init; }

    [Inject]
    private IDispatcher Dispatcher { get; init; }

    private bool validationSuccess;
    private string[] errors = [];
    private MudForm form;

    protected override async Task OnInitializedAsync()
    {
        //var client = new HttpClient { BaseAddress = new Uri($"http://localhost:7111") };
        //var stream = await client.GetStreamAsync("swagger/Alpha%20Release/swagger.json");
        //var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

        base.OnInitializedAsync();
    }

    private static HashSet<string> methodsToExclude =
    [
        nameof(IStructuralAnalysisApiClientV1.CreateModelAsync),
        //nameof(IStructuralAnalysisApiClientV1.GetElement1dsAsync),
        nameof(IStructuralAnalysisApiClientV1.GetModelAsync),
        nameof(IStructuralAnalysisApiClientV1.GetElement1dAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetModelResultsAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetModelsAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetMomentDiagramAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetMomentLoadsAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetNodeResultsAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetShearDiagramAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetSingleElement1dAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetSingleNodeResultAsync),
    ];

    static StructuralApiComponent()
    {
        ClientMethods = typeof(IStructuralAnalysisApiClientV1)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(
                m =>
                    m.GetParameters().LastOrDefault() is var x
                    && x is not null
                    && x.ParameterType != typeof(CancellationToken)
                    && !methodsToExclude.Contains(m.Name)
            )
            .OrderBy(m => m.Name)
            .Select(
                m =>
                    new ApiClientMethodInfo(
                        m,
                        GetHttpMethodForMethodInfo(m),
                        GetPrimaryElementType(m)
                    )
            )
            .ToArray();
    }

    private static readonly ApiClientMethodInfo[] ClientMethods;

    protected override void OnInitialized()
    {
        //this.EditorState.Select(s => s.EditorState[this.CanvasId]);
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
                state.CurrentlySelectedFieldInfo.SetValue(c.SelectedObjects[0].Id);

                if (state.ElementRefs.Count >= state.CurrentlySelectedFieldInfo.FieldIndex + 2)
                {
                    await this.State
                        .Value
                        .ElementRefs[state.CurrentlySelectedFieldInfo.FieldIndex + 1]
                        .FocusAsync();
                }
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

            if (this.State.Value.ElementRefs?.FirstOrDefault() is ElementReference firstRef)
            {
                await firstRef.FocusAsync();
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void SelectMethod(ApiClientMethodInfo method)
    {
        this.Dispatcher.Dispatch(new ApiClientMethodSelected(this.ModelId, method));
    }

    private async Task HandleSubmit()
    {
        await this.form.Validate();

        if (!this.validationSuccess)
        {
            return;
        }

        var state = this.State.Value;
        var selectionInfos = state.SelectionInfo;
        object?[] parameters = new object?[selectionInfos.Length];

        for (int i = 0; i < selectionInfos.Length; i++)
        {
            if (selectionInfos[i] is ISimpleSelectionInfo simpleSelectionInfo)
            {
                parameters[i] = simpleSelectionInfo.Value;
            }
            else if (selectionInfos[i] is ComplexFieldSelectionInfo complex)
            {
                var x = complex.ToJsonObject()?.ToJsonString();

                try
                {
                    parameters[i] = JsonSerializer.Deserialize(
                        complex.ToJsonObject(),
                        complex.FieldType
                    );
                }
                catch (JsonException ex)
                {
                    this.Snackbar.Add($"Error deserializing JSON: {ex.Message}", Severity.Error);
                    // Handle JSON deserialization error
                    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                    return;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        var result = state
            .SelectedMethod
            .Value
            .MethodInfo
            .Invoke(this.StructuralAnalysisApiAlphaClient, parameters);

        if (result is Task t)
        {
            await t.ConfigureAwait(false);
            result = ((dynamic)t).Result;
        }

        if (result is Result resultContract)
        {
            result = ((dynamic)resultContract).Value;
        }

        if (result is IModelEntity modelEntity)
        {
            if (state.SelectedMethod.Value.Http == Http.Delete)
            {
                this.Dispatcher.Dispatch(
                    new ModelEntityDeleted()
                    {
                        ModelEntity = modelEntity,
                        EntityType = state.SelectedMethod.Value.PrimaryElementType,
                        HandledByServer = true
                    }
                );
            }
            else if (state.SelectedMethod.Value.Http == Http.Post)
            {
                this.Dispatcher.Dispatch(
                    new ModelEntityCreated() { ModelEntity = modelEntity, HandledByServer = true }
                );
            }
        }

        //if (result is BeamOsEntityContractBase contract)
        //{
        //    await this.AddEntityContractToEditorCommandHandler.ExecuteAsync(
        //        new(this.CanvasId, contract)
        //    );
        //}
        //else if (result is IEnumerable<BeamOsEntityContractBase> entities && entities.Any())
        //{
        //    IClientCommand command = entities.First() switch
        //    {
        //        ShearDiagramResponse
        //            => new AddEntitiesToEditorCommand<ShearDiagramResponse>(
        //                this.CanvasId,
        //                entities.Cast<ShearDiagramResponse>()
        //            ),
        //        MomentDiagramResponse
        //            => new AddEntitiesToEditorCommand<MomentDiagramResponse>(
        //                this.CanvasId,
        //                entities.Cast<MomentDiagramResponse>()
        //            ),
        //        _ => throw new NotImplementedException(),
        //    };
        //    await this.GenericCommandHandler.ExecuteAsync(command);
        //}

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

    private static string GetHttpMethodForMethodInfo(MethodInfo methodInfo)
    {
        if (
            methodInfo.Name.StartsWith("Create", StringComparison.Ordinal)
            || methodInfo.Name.StartsWith("Run", StringComparison.Ordinal)
        )
        {
            return Http.Post;
        }
        else if (methodInfo.Name.StartsWith("Delete", StringComparison.Ordinal))
        {
            return Http.Delete;
        }
        else if (
            methodInfo.Name.StartsWith("Patch", StringComparison.Ordinal)
            || methodInfo.Name.StartsWith("Update", StringComparison.Ordinal)
        )
        {
            return Http.Patch;
        }
        else if (methodInfo.Name.StartsWith("Get", StringComparison.Ordinal))
        {
            return Http.Get;
        }
        throw new Exception("todo");
    }

    private static string GetPrimaryElementType(MethodInfo methodInfo)
    {
        if (methodInfo.Name.Contains(nameof(Element1d), StringComparison.Ordinal))
        {
            return nameof(Element1d);
        }
        else if (methodInfo.Name.Contains(nameof(Node), StringComparison.Ordinal))
        {
            return nameof(Node);
        }
        else if (methodInfo.Name.Contains(nameof(PointLoad), StringComparison.Ordinal))
        {
            return nameof(PointLoad);
        }
        else if (methodInfo.Name.Contains(nameof(MomentLoad), StringComparison.Ordinal))
        {
            return nameof(MomentLoad);
        }
        else if (methodInfo.Name.Contains(nameof(Material), StringComparison.Ordinal))
        {
            return nameof(Material);
        }
        else if (methodInfo.Name.Contains(nameof(SectionProfile), StringComparison.Ordinal))
        {
            return nameof(SectionProfile);
        }
        else if (methodInfo.Name.Contains(nameof(Model), StringComparison.Ordinal))
        {
            return nameof(Model);
        }
        else if (methodInfo.Name.Contains(nameof(ResultSet), StringComparison.Ordinal))
        {
            return nameof(ResultSet);
        }
        else if (
            methodInfo.Name.Contains("OpenSees", StringComparison.Ordinal)
            || methodInfo.Name.Contains("DirectStiffness", StringComparison.Ordinal)
        )
        {
            return nameof(Model);
        }
        throw new Exception($"could not find primary element type for method {methodInfo.Name}");
    }

    private static Color GetChipColor(string http)
    {
        return http switch
        {
            Http.Delete => Color.Error,
            Http.Get => Color.Info,
            Http.Patch => Color.Warning,
            Http.Post => Color.Success,
            Http.Put => Color.Warning,
            _ => throw new Exception("todo")
        };
    }

    private void GoBack()
    {
        this.Dispatcher.Dispatch(new ApiClientMethodSelected(this.ModelId, null));
    }
}

public readonly record struct ApiClientMethodInfo(
    MethodInfo MethodInfo,
    string Http,
    string PrimaryElementType
);

[FeatureState]
public record class StructuralApiClientState(
    string? ModelId,
    ApiClientMethodInfo? SelectedMethod,
    ISelectionInfo[]? SelectionInfo,
    List<Lazy<ElementReference>>? LazyElementRefs,
    List<ElementReference>? ElementRefs,
    SelectionInfo.FieldInfo? CurrentlySelectedFieldInfo
)
{
    public StructuralApiClientState()
        : this(null, null, null, null, null, null) { }
}

public static class ApiClientComponentReducers
{
    [ReducerMethod]
    public static StructuralApiClientState ReduceElementReferencesEvaluated(
        StructuralApiClientState state,
        ElementReferencesEvaluated action
    )
    {
        return state with { LazyElementRefs = null, ElementRefs = action.ElementReferences };
    }

    [ReducerMethod]
    public static StructuralApiClientState ReduceFormInputCreated(
        StructuralApiClientState state,
        FormInputCreated action
    )
    {
        return state with
        {
            LazyElementRefs = state.LazyElementRefs is null
                ? [action.LazyReference]
                : new(state.LazyElementRefs) { action.LazyReference }
        };
    }

    [ReducerMethod]
    public static StructuralApiClientState ReduceFieldSelectedAction(
        StructuralApiClientState state,
        FieldSelected action
    )
    {
        return state with { CurrentlySelectedFieldInfo = action.FieldInfo };
    }

    [ReducerMethod]
    public static StructuralApiClientState ReduceApiClientMethodSelectedAction(
        StructuralApiClientState state,
        ApiClientMethodSelected action
    )
    {
        if (action.MethodInfo is null)
        {
            return state with
            {
                SelectedMethod = null,
                SelectionInfo = null,
                CurrentlySelectedFieldInfo = null,
                LazyElementRefs = null,
                ElementRefs = null
            };
        }

        EditableSelectionInfoFactory selectionInfoFactory = new(action.ModelId);
        return state with
        {
            SelectedMethod = action.MethodInfo,
            SelectionInfo = action
                .MethodInfo
                .Value
                .MethodInfo
                .GetParameters()
                .Select(p => selectionInfoFactory.Create(null, p.ParameterType, p.Name, true, 1))
                .ToArray()
        };
    }
}
