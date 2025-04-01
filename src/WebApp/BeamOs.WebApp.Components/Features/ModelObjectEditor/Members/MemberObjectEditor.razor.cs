using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Members;

public partial class MemberObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<Element1dObjectEditorState> state,
    PutElement1dSimpleCommandHandler putElement1dCommandHandler,
    CreateElement1dClientCommandHandler createElement1dCommandHandler,
    DeleteElement1dSimpleCommandHandler deleteElement1dCommandHandler
) : FluxorComponent
{
    private readonly Element1dModel element1d = new();

    private int Element1dId
    {
        get => this.element1d.Id;
        set
        {
            if (value > 0)
            {
                this.UpdateFromElement1dId(value);
            }
            this.element1d.Id = value;
        }
    }

    private const int ResultLimit = 50;

    private string Element1dIdText =>
        this.Element1dId == 0 ? "New Element" : this.Element1dId.ToString();

    [Parameter]
    public required UnitSettingsContract UnitSettings { get; set; }

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        // putElement1dCommandHandler.IsLoadingChanged += this.PutElement1dCommandHandler_IsLoadingChanged;
        // createElement1dCommandHandler.IsLoadingChanged += this.PutElement1dCommandHandler_IsLoadingChanged;
        // deleteElement1dCommandHandler.IsLoadingChanged += this.PutElement1dCommandHandler_IsLoadingChanged;

        base.OnInitialized();
        /*this.SubscribeToAction<PutElement1dClientCommand>(command =>*/
        /*{*/
        /*    if (command.New is null)*/
        /*    {*/
        /*        return;*/
        /*    }*/
        /**/
        /*    if (command.New.Id == this.SelectedObject?.Id)*/
        /*    {*/
        /*        this.UpdateFromElement1dResponse(command.New);*/
        /*    }*/
        /*});*/
    }

    // private void PutElement1dCommandHandler_IsLoadingChanged(object? sender, bool e) =>
    //     dispatcher.Dispatch(new Element1dObjectEditorIsLoading(e));

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this.SelectedObject is not null && this.SelectedObject.TypeName == "Element1d")
        {
            this.UpdateFromElement1dId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromElement1dId(int nodeId)
    {
        var response = editorState.Value.CachedModelResponse.Element1ds[nodeId];
        this.UpdateFromElement1dResponse(response);
    }

    private void UpdateFromElement1dResponse(Element1dResponse response)
    {
        this.element1d.Id = response.Id;
        this.element1d.ModelId = response.ModelId;
        this.element1d.StartNodeId = response.StartNodeId;
        this.element1d.EndNodeId = response.EndNodeId;
        this.element1d.MaterialId = response.MaterialId;
        this.element1d.SectionProfileId = response.SectionProfileId;
        this.element1d.SectionProfileRotation = response
            .SectionProfileRotation
            .As(response.SectionProfileRotation.Unit);
        this.element1d.Metadata = response.Metadata;
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(new FieldSelected(new(fieldName, fieldNum, setValue)));
    }

    private async Task Delete()
    {
        ModelEntityCommand command = new() { ModelId = this.ModelId, Id = this.element1d.Id };
        await deleteElement1dCommandHandler.ExecuteAsync(command);
    }

    private async Task Submit()
    {
        Element1dData nodeData =
            new()
            {
                StartNodeId = this.element1d.StartNodeId,
                EndNodeId = this.element1d.EndNodeId,
                MaterialId = this.element1d.MaterialId,
                SectionProfileId = this.element1d.SectionProfileId,
                SectionProfileRotation = new(
                    this.element1d.SectionProfileRotation ?? 0,
                    this.UnitSettings.AngleUnit
                ),
                Metadata = this.element1d.Metadata
            };

        if (this.element1d.Id == 0)
        {
            CreateElement1dClientCommand command = new(nodeData) { ModelId = this.ModelId };
            await createElement1dCommandHandler.ExecuteAsync(command);
        }
        else
        {
            PutElement1dCommand command =
                new()
                {
                    Id = this.element1d.Id,
                    ModelId = this.ModelId,
                    Body = nodeData
                };

            await putElement1dCommandHandler.ExecuteAsync(command);
        }
    }

    private static readonly int[] NullInt = [0];

    private Task<IEnumerable<int>> Element1dIds(string searchText, CancellationToken ct)
    {
        return GetPossibleIdsFromSubstring(
            searchText,
            editorState.Value.CachedModelResponse.Element1ds.Keys,
            true
        );
    }

    private Task<IEnumerable<int>> NodeIds(string searchText, CancellationToken ct)
    {
        return GetPossibleIdsFromSubstring(
            searchText,
            editorState.Value.CachedModelResponse.Nodes.Keys,
            false
        );
    }

    private Task<IEnumerable<int>> MaterialIds(string searchText, CancellationToken ct)
    {
        return GetPossibleIdsFromSubstring(
            searchText,
            editorState.Value.CachedModelResponse.Materials.Keys,
            false
        );
    }

    private Task<IEnumerable<int>> SectionProfileIds(string searchText, CancellationToken ct)
    {
        return GetPossibleIdsFromSubstring(
            searchText,
            editorState.Value.CachedModelResponse.SectionProfiles.Keys,
            false
        );
    }

    private static Task<IEnumerable<int>> GetPossibleIdsFromSubstring(
        string searchText,
        IEnumerable<int> keys,
        bool includeNew = false
    )
    {
        IEnumerable<int> result;
        if (includeNew)
        {
            result = NullInt.Concat(GetPossibleIdsFromSubstring(searchText, keys));
        }
        else
        {
            result = GetPossibleIdsFromSubstring(searchText, keys);
        }

        return Task.FromResult(result);
    }

    private static IEnumerable<int> GetPossibleIdsFromSubstring(
        string searchText,
        IEnumerable<int> keys
    )
    {
        if (!int.TryParse(searchText, out int subInt) || subInt <= 0)
        {
            return keys;
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return keys.Where(k => GetPrefix(k, subIntLength) == subInt);
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

    public class Element1dModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public int StartNodeId { get; set; }
        public int EndNodeId { get; set; }
        public int MaterialId { get; set; }
        public int SectionProfileId { get; set; }
        public double? SectionProfileRotation { get; set; } = 0;
        public Dictionary<string, string>? Metadata { get; set; }
    }
}

[FeatureState]
public record Element1dObjectEditorState(bool IsLoading)
{
    public Element1dObjectEditorState()
        : this(false) { }
}

public record struct Element1dObjectEditorIsLoading(bool IsLoading);

public static class Element1dObjectEditorStateReducers
{
    [ReducerMethod]
    public static Element1dObjectEditorState Reducer(
        Element1dObjectEditorState state,
        Element1dObjectEditorIsLoading action
    ) => state with { IsLoading = action.IsLoading };
}
