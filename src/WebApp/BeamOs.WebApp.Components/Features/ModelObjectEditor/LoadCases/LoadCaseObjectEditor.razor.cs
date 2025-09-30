using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.ModelObjectEditor.MomentLoads;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.LoadCases;

public partial class LoadCaseObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<LoadCaseObjectEditorState> state,
    PutLoadCaseSimpleCommandHandler putLoadCaseCommandHandler,
    CreateLoadCaseClientCommandHandler createLoadCaseCommandHandler,
    DeleteLoadCaseSimpleCommandHandler deleteLoadCaseCommandHandler
) : FluxorComponent
{
    private readonly LoadCaseModel loadCase = new();

    private int LoadCaseId
    {
        get => this.loadCase.Id;
        set
        {
            if (value > 0)
            {
                this.UpdateFromLoadCaseId(value);
            }
            this.loadCase.Id = value;
        }
    }

    private const int ResultLimit = 50;

    private string LoadCaseIdText =>
        this.LoadCaseId == 0 ? "New Section Profile" : this.LoadCaseId.ToString();

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        // putLoadCaseCommandHandler.IsLoadingChanged += this.PutLoadCaseCommandHandler_IsLoadingChanged;
        // createLoadCaseCommandHandler.IsLoadingChanged += this.PutLoadCaseCommandHandler_IsLoadingChanged;
        // deleteLoadCaseCommandHandler.IsLoadingChanged += this.PutLoadCaseCommandHandler_IsLoadingChanged;

        base.OnInitialized();
        this.SubscribeToAction<PutLoadCaseClientCommand>(command =>
        {
            if (command.New is null)
            {
                return;
            }

            if (command.New.Id == this.SelectedObject?.Id)
            {
                this.UpdateFromLoadCaseResponse(command.New);
            }
        });
    }

    // private void PutLoadCaseCommandHandler_IsLoadingChanged(object? sender, bool e) =>
    //     dispatcher.Dispatch(new LoadCaseObjectEditorIsLoading(e));

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (
            this.SelectedObject is not null
            && this.SelectedObject.ObjectType == BeamOsObjectType.LoadCase
        )
        {
            this.UpdateFromLoadCaseId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromLoadCaseId(int loadCaseId)
    {
        var response = editorState.Value.CachedModelResponse.LoadCases[loadCaseId];
        this.UpdateFromLoadCaseResponse(response);
    }

    private void UpdateFromLoadCaseResponse(LoadCase response)
    {
        this.loadCase.Id = response.Id;
        this.loadCase.Name = response.Name;
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(new FieldSelected(new(fieldName, fieldNum, setValue)));
    }

    private async Task Delete()
    {
        ModelEntityCommand command = new() { ModelId = this.ModelId, Id = this.loadCase.Id };
        await deleteLoadCaseCommandHandler.ExecuteAsync(command);
    }

    private async Task Submit()
    {
        LoadCase loadCaseData = new() { Id = this.loadCase.Id, Name = this.loadCase.Name };

        if (this.loadCase.Id == 0)
        {
            CreateLoadCaseClientCommand command = new(loadCaseData) { ModelId = this.ModelId };
            await createLoadCaseCommandHandler.ExecuteAsync(command);
        }
        else
        {
            await putLoadCaseCommandHandler.ExecuteAsync(loadCaseData);
        }
    }

    private static readonly int[] NullInt = [0];

    private Task<IEnumerable<int>> LoadCaseIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(
                NullInt.Concat(editorState.Value.CachedModelResponse.LoadCases.Keys)
            );
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            NullInt.Concat(
                editorState.Value.CachedModelResponse.LoadCases.Keys.Where(k =>
                    GetPrefix(k, subIntLength) == subInt
                )
            )
        );
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

    public class LoadCaseModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

[FeatureState]
public record LoadCaseObjectEditorState(bool IsLoading)
{
    public LoadCaseObjectEditorState()
        : this(false) { }
}

public record struct LoadCaseObjectEditorIsLoading(bool IsLoading);

public static class LoadCaseObjectEditorStateReducers
{
    [ReducerMethod]
    public static LoadCaseObjectEditorState Reducer(
        LoadCaseObjectEditorState state,
        LoadCaseObjectEditorIsLoading action
    ) => state with { IsLoading = action.IsLoading };
}
