using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.LoadCombinations;

public partial class LoadCombinationObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<LoadCombinationObjectEditorState> state,
    PutLoadCombinationSimpleCommandHandler putLoadCombinationCommandHandler,
    CreateLoadCombinationClientCommandHandler createLoadCombinationCommandHandler,
    DeleteLoadCombinationSimpleCommandHandler deleteLoadCombinationCommandHandler
) : FluxorComponent
{
    private readonly LoadCombinationModel loadCombination = new();

    private int LoadCombinationId
    {
        get => this.loadCombination.Id;
        set
        {
            if (value > 0)
            {
                this.UpdateFromLoadCombinationId(value);
            }
            this.loadCombination.Id = value;
        }
    }

    private const int ResultLimit = 50;

    private string LoadCombinationIdText =>
        this.LoadCombinationId == 0 ? "New Section Profile" : this.LoadCombinationId.ToString();

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        // putLoadCombinationCommandHandler.IsLoadingChanged += this.PutLoadCombinationCommandHandler_IsLoadingChanged;
        // createLoadCombinationCommandHandler.IsLoadingChanged += this.PutLoadCombinationCommandHandler_IsLoadingChanged;
        // deleteLoadCombinationCommandHandler.IsLoadingChanged += this.PutLoadCombinationCommandHandler_IsLoadingChanged;

        base.OnInitialized();
        this.SubscribeToAction<PutLoadCombinationClientCommand>(command =>
        {
            if (command.New is null)
            {
                return;
            }

            if (command.New.Id == this.SelectedObject?.Id)
            {
                this.UpdateFromLoadCombinationResponse(command.New);
            }
        });
    }

    // private void PutLoadCombinationCommandHandler_IsLoadingChanged(object? sender, bool e) =>
    //     dispatcher.Dispatch(new LoadCombinationObjectEditorIsLoading(e));

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (
            this.SelectedObject is not null
            && this.SelectedObject.ObjectType == BeamOsObjectType.LoadCombination
        )
        {
            this.UpdateFromLoadCombinationId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromLoadCombinationId(int loadCombinationId)
    {
        var response = editorState.Value.CachedModelResponse.LoadCombinations[loadCombinationId];
        this.UpdateFromLoadCombinationResponse(response);
    }

    private void UpdateFromLoadCombinationResponse(LoadCombination response)
    {
        this.loadCombination.Id = response.Id;
        this.loadCombination.LoadCaseFactors.Clear();
        foreach (KeyValuePair<int, double> responseLoadCaseFactor in response.LoadCaseFactors)
        {
            this.loadCombination.LoadCaseFactors.Add(
                (responseLoadCaseFactor.Key, responseLoadCaseFactor.Value)
            );
        }
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(new FieldSelected(new(fieldName, fieldNum, setValue)));
    }

    private async Task Delete()
    {
        ModelEntityCommand command = new() { ModelId = this.ModelId, Id = this.loadCombination.Id };
        await deleteLoadCombinationCommandHandler.ExecuteAsync(command);
    }

    private async Task Submit()
    {
        LoadCombination loadCombinationData = new()
        {
            Id = this.loadCombination.Id,
            LoadCaseFactors = this.loadCombination.LoadCaseFactors.ToDictionary(),
        };

        if (this.loadCombination.Id == 0)
        {
            CreateLoadCombinationClientCommand command = new(loadCombinationData)
            {
                ModelId = this.ModelId,
            };
            await createLoadCombinationCommandHandler.ExecuteAsync(command);
        }
        else
        {
            await putLoadCombinationCommandHandler.ExecuteAsync(loadCombinationData);
        }
    }

    private static readonly int[] NullInt = [0];

    private Task<IEnumerable<int>> LoadCombinationIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(
                NullInt.Concat(editorState.Value.CachedModelResponse.LoadCombinations.Keys)
            );
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            NullInt.Concat(
                editorState.Value.CachedModelResponse.LoadCombinations.Keys.Where(k =>
                    GetPrefix(k, subIntLength) == subInt
                )
            )
        );
    }

    private Task<IEnumerable<LoadCase>> LoadCases(string searchText, CancellationToken ct)
    {
        IEnumerable<LoadCase> result = editorState.Value.CachedModelResponse.LoadCases.Values;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            result = result.Where(lc =>
                lc.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            );
        }
        return Task.FromResult(result.OrderBy(lc => lc.Name).AsEnumerable());
    }

    // Helper method to get the number of digits in an integer
    private static int GetNumberOfDigits(int number)
    {
        if (number == 0)
            return 1; // Edge case for 0
        return (int)Math.Floor(Math.Log10(Math.Abs(number))) + 1;
    }

    // Helper method to get the prefix of a number with a specified length
    private static int GetPrefix(int number, int prefixLength)
    {
        int numberOfDigits = GetNumberOfDigits(number);
        if (prefixLength >= numberOfDigits)
        {
            return number; // The entire number is the prefix
        }
        return number / (int)Math.Pow(10, numberOfDigits - prefixLength);
    }

    private void LoadCaseChanged(int loadCaseFactorIndex, LoadCase loadCase)
    {
        this.loadCombination.LoadCaseFactors[loadCaseFactorIndex] = (
            loadCase.Id,
            this.loadCombination.LoadCaseFactors[loadCaseFactorIndex].Item2
        );
    }

    private void LoadCaseValueChanged(int loadCaseFactorIndex, double value)
    {
        this.loadCombination.LoadCaseFactors[loadCaseFactorIndex] = (
            this.loadCombination.LoadCaseFactors[loadCaseFactorIndex].Item1,
            value
        );
    }

    private LoadCase GetLoadCase(int loadCaseId)
    {
        return editorState.Value.CachedModelResponse.LoadCases[loadCaseId];
    }

    public class LoadCombinationModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public List<(int, double)> LoadCaseFactors { get; } = [];
    }
}

[FeatureState]
public record LoadCombinationObjectEditorState(bool IsLoading)
{
    public LoadCombinationObjectEditorState()
        : this(false) { }
}

public record struct LoadCombinationObjectEditorIsLoading(bool IsLoading);

public static class LoadCombinationObjectEditorStateReducers
{
    [ReducerMethod]
    public static LoadCombinationObjectEditorState Reducer(
        LoadCombinationObjectEditorState state,
        LoadCombinationObjectEditorIsLoading action
    ) => state with { IsLoading = action.IsLoading };
}
