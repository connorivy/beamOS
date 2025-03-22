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
        this.SubscribeToAction<PutElement1dClientCommand>(command =>
        {
            if (command.New is null)
            {
                return;
            }

            if (command.New.Id == this.SelectedObject?.Id)
            {
                this.UpdateFromElement1dResponse(command.New);
            }
        });
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
        var thisLengthUnit = this.UnitSettings.LengthUnit.MapToLengthUnit();
        var lengthUnit = response.LocationPoint.LengthUnit.MapToLengthUnit();
        this.element1d.Id = response.Id;
        this.element1d.ModelId = response.ModelId;
        this.element1d.LocationPoint.X = new Length(response.LocationPoint.X, lengthUnit).As(
            thisLengthUnit
        );
        this.element1d.LocationPoint.Y = new Length(response.LocationPoint.Y, lengthUnit).As(
            thisLengthUnit
        );
        this.element1d.LocationPoint.Z = new Length(response.LocationPoint.Z, lengthUnit).As(
            thisLengthUnit
        );
        this.element1d.LocationPoint.LengthUnit = response.LocationPoint.LengthUnit;
        this.element1d.Restraint.CanTranslateAlongX = response.Restraint.CanTranslateAlongX;
        this.element1d.Restraint.CanTranslateAlongY = response.Restraint.CanTranslateAlongY;
        this.element1d.Restraint.CanTranslateAlongZ = response.Restraint.CanTranslateAlongZ;
        this.element1d.Restraint.CanRotateAboutX = response.Restraint.CanRotateAboutX;
        this.element1d.Restraint.CanRotateAboutY = response.Restraint.CanRotateAboutY;
        this.element1d.Restraint.CanRotateAboutZ = response.Restraint.CanRotateAboutZ;
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
                SectionProfileRotation = this.element1d.SectionProfileRotation,
                Metadata = this.element1d.Metadata
            };

        if (this.element1d.Id == 0)
        {
            CreateElement1dCommand command =
                new()
                {
                    Body = new()
                    {
                        StartNodeId = this.element1d.StartNodeId,
                        EndNodeId = this.element1d.EndNodeId,
                        MaterialId = this.element1d.MaterialId,
                        SectionProfileId = this.element1d.SectionProfileId,
                        SectionProfileRotation = this.element1d.SectionProfileRotation,
                    },
                    ModelId = this.ModelId
                };
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
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(
                NullInt.Concat(editorState.Value.CachedModelResponse.Element1ds.Keys)
            );
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            NullInt.Concat(
                editorState
                    .Value
                    .CachedModelResponse
                    .Element1ds
                    .Keys
                    .Where(k => GetPrefix(k, subIntLength) == subInt)
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

    // protected override ValueTask DisposeAsyncCore(bool disposing)
    // {
    //     putElement1dCommandHandler.IsLoadingChanged -= this.PutElement1dCommandHandler_IsLoadingChanged;
    //     createElement1dCommandHandler.IsLoadingChanged -= this.PutElement1dCommandHandler_IsLoadingChanged;
    //     deleteElement1dCommandHandler.IsLoadingChanged -= this.PutElement1dCommandHandler_IsLoadingChanged;
    //     return base.DisposeAsyncCore(disposing);
    // }

    public class Element1dModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public int StartNodeId { get; set; }
        public int EndNodeId { get; set; }
        public int MaterialId { get; set; }
        public int SectionProfileId { get; set; }
        public AngleContract SectionProfileRotation { get; set; }
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
