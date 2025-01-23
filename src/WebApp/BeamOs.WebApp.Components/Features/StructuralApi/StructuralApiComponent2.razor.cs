using System.Reflection;
using System.Text.Json;
using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.StructuralApi;

public partial class StructuralApiComponent2 : FluxorComponent
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
    private IState<StructuralApiClientState2> State { get; init; }

    [Inject]
    private IDispatcher Dispatcher { get; init; }

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
    //nameof(IStructuralAnalysisApiClientV1.GetModelResultsAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetModelsAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetMomentDiagramAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetMomentLoadsAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetNodeResultsAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetShearDiagramAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetSingleElement1dAsync),
    //nameof(IStructuralAnalysisApiClientV1.GetSingleNodeResultAsync),
    ];

    static StructuralApiComponent2()
    {
        methods = typeof(IStructuralAnalysisApiClientV1)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(
                m =>
                    m.GetParameters().LastOrDefault() is var x
                    && x is not null
                    && x.ParameterType != typeof(CancellationToken)
                    && !methodsToExclude.Contains(m.Name)
            )
            .OrderBy(m => m.Name)
            .ToArray();
    }

    private static readonly MethodInfo[] methods;

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

    private void SelectMethod(MethodInfo method)
    {
        this.Dispatcher.Dispatch(new ApiClientMethodSelected(this.ModelId, method));
    }

    private async Task HandleSubmit()
    {
        var selectionInfos = this.State.Value.SelectionInfo;
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

        var result = this.State
            .Value
            .SelectedMethod
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
            Dispatcher.Dispatch(
                new ModelEntityCreated() { ModelEntity = modelEntity, HandledByServer = true }
            );
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
        else if (
            methodInfo.Name.StartsWith("Patch", StringComparison.Ordinal)
            || methodInfo.Name.StartsWith("Update", StringComparison.Ordinal)
        )
        {
            return (Color.Warning, "PATCH");
        }
        else if (methodInfo.Name.StartsWith("Get", StringComparison.Ordinal))
        {
            return (Color.Info, "GET");
        }
        throw new Exception("todo");
    }

    private void GoBack()
    {
        this.Dispatcher.Dispatch(new ApiClientMethodSelected(this.ModelId, null));
    }
}

public readonly record struct ModelEntityCreated : IBeamOsClientCommand
{
    public Guid Id { get; }
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public IModelEntity ModelEntity { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new ModelEntityDeleted() { ModelEntity = this.ModelEntity };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        throw new NotImplementedException();
}

public readonly record struct ModelEntityDeleted : IBeamOsClientCommand
{
    public Guid Id { get; }
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public IModelEntity ModelEntity { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new ModelEntityCreated() { ModelEntity = this.ModelEntity };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        throw new NotImplementedException();
}

[FeatureState]
public record class StructuralApiClientState2(
    string? ModelId,
    MethodInfo? SelectedMethod,
    ISelectionInfo[]? SelectionInfo,
    List<Lazy<ElementReference>>? LazyElementRefs,
    List<ElementReference>? ElementRefs,
    SelectionInfo.FieldInfo? CurrentlySelectedFieldInfo
)
{
    public StructuralApiClientState2()
        : this(null, null, null, null, null, null) { }
}

public readonly record struct ElementReferencesEvaluated(List<ElementReference> ElementReferences);

public static class ElementReferencesEvaluatedActionReducer
{
    [ReducerMethod]
    public static StructuralApiClientState2 ReduceElementReferencesEvaluated(
        StructuralApiClientState2 state,
        ElementReferencesEvaluated action
    )
    {
        return state with { LazyElementRefs = null, ElementRefs = action.ElementReferences };
    }
}

public static class FormInputCreatedActionReducer
{
    [ReducerMethod]
    public static StructuralApiClientState2 ReduceFormInputCreated(
        StructuralApiClientState2 state,
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
}

public static class FieldSelectedActionReducer
{
    [ReducerMethod]
    public static StructuralApiClientState2 ReduceFieldSelectedAction(
        StructuralApiClientState2 state,
        FieldSelected action
    )
    {
        return state with { CurrentlySelectedFieldInfo = action.FieldInfo };
    }
}

public readonly record struct ApiClientMethodSelected(Guid ModelId, MethodInfo? MethodInfo);

public static class ApiClientMethodSelectedActionReducer2
{
    [ReducerMethod]
    public static StructuralApiClientState2 ReduceApiClientMethodSelectedAction(
        StructuralApiClientState2 state,
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
                .GetParameters()
                .Select(p => selectionInfoFactory.Create(null, p.ParameterType, p.Name, true, 1))
                .ToArray()
        };
    }
}
