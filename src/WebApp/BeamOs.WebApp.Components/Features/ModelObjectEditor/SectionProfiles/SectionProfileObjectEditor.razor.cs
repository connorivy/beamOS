using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.SectionProfiles;

public partial class SectionProfileObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<SectionProfileObjectEditorState> state,
    PutSectionProfileSimpleCommandHandler putSectionProfileCommandHandler,
    CreateSectionProfileClientCommandHandler createSectionProfileCommandHandler,
    DeleteSectionProfileSimpleCommandHandler deleteSectionProfileCommandHandler
) : FluxorComponent
{
    private readonly SectionProfileModel sectionProfile = new();

    private int SectionProfileId
    {
        get => this.sectionProfile.Id;
        set
        {
            if (value > 0)
            {
                this.UpdateFromSectionProfileId(value);
            }
            this.sectionProfile.Id = value;
        }
    }

    private const int ResultLimit = 50;

    private string SectionProfileIdText =>
        this.SectionProfileId == 0 ? "New Section Profile" : this.SectionProfileId.ToString();

    [Parameter]
    public required UnitSettingsContract UnitSettings { get; set; }

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        // putSectionProfileCommandHandler.IsLoadingChanged += this.PutSectionProfileCommandHandler_IsLoadingChanged;
        // createSectionProfileCommandHandler.IsLoadingChanged += this.PutSectionProfileCommandHandler_IsLoadingChanged;
        // deleteSectionProfileCommandHandler.IsLoadingChanged += this.PutSectionProfileCommandHandler_IsLoadingChanged;

        base.OnInitialized();
        this.SubscribeToAction<PutSectionProfileClientCommand>(command =>
        {
            if (command.New is null)
            {
                return;
            }

            if (command.New.Id == this.SelectedObject?.Id)
            {
                this.UpdateFromSectionProfileResponse(command.New);
            }
        });
    }

    // private void PutSectionProfileCommandHandler_IsLoadingChanged(object? sender, bool e) =>
    //     dispatcher.Dispatch(new SectionProfileObjectEditorIsLoading(e));

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this.SelectedObject is not null && this.SelectedObject.TypeName == "SectionProfile")
        {
            this.UpdateFromSectionProfileId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromSectionProfileId(int sectionProfileId)
    {
        var response = editorState.Value.CachedModelResponse.SectionProfiles[sectionProfileId];
        this.UpdateFromSectionProfileResponse(response);
    }

    private void UpdateFromSectionProfileResponse(SectionProfileResponse response)
    {
        var areaUnit = response.AreaUnit.MapToAreaUnit();
        var momentOfInertiaUnit = response.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit();
        var thisAreaUnit = this.UnitSettings.AreaUnit.MapToAreaUnit();
        var thisMomentOfInertiaUnit = this.UnitSettings
            .AreaMomentOfInertiaUnit
            .MapToAreaMomentOfInertiaUnit();

        this.sectionProfile.Id = response.Id;
        this.sectionProfile.ModelId = response.ModelId;
        this.sectionProfile.Area = new Area(response.Area, areaUnit).As(thisAreaUnit);
        this.sectionProfile.StrongAxisMomentOfInertia = new AreaMomentOfInertia(
            response.StrongAxisMomentOfInertia,
            momentOfInertiaUnit
        ).As(thisMomentOfInertiaUnit);
        this.sectionProfile.WeakAxisMomentOfInertia = new AreaMomentOfInertia(
            response.WeakAxisMomentOfInertia,
            momentOfInertiaUnit
        ).As(thisMomentOfInertiaUnit);
        this.sectionProfile.PolarMomentOfInertia = new AreaMomentOfInertia(
            response.PolarMomentOfInertia,
            momentOfInertiaUnit
        ).As(thisMomentOfInertiaUnit);
        this.sectionProfile.StrongAxisShearArea = new Area(
            response.StrongAxisShearArea,
            areaUnit
        ).As(thisAreaUnit);
        this.sectionProfile.WeakAxisShearArea = new Area(response.WeakAxisShearArea, areaUnit).As(
            thisAreaUnit
        );
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(new FieldSelected(new(fieldName, fieldNum, setValue)));
    }

    private async Task Delete()
    {
        ModelEntityCommand command = new() { ModelId = this.ModelId, Id = this.sectionProfile.Id };
        await deleteSectionProfileCommandHandler.ExecuteAsync(command);
    }

    private async Task Submit()
    {
        SectionProfileData sectionProfileData =
            new()
            {
                AreaUnit = this.UnitSettings.AreaUnit,
                AreaMomentOfInertiaUnit = this.UnitSettings.AreaMomentOfInertiaUnit,
                Area = this.sectionProfile.Area.Value,
                StrongAxisMomentOfInertia = this.sectionProfile.StrongAxisMomentOfInertia.Value,
                WeakAxisMomentOfInertia = this.sectionProfile.WeakAxisMomentOfInertia.Value,
                PolarMomentOfInertia = this.sectionProfile.PolarMomentOfInertia.Value,
                StrongAxisShearArea = this.sectionProfile.StrongAxisShearArea.Value,
                WeakAxisShearArea = this.sectionProfile.WeakAxisShearArea.Value
            };

        if (this.sectionProfile.Id == 0)
        {
            CreateSectionProfileClientCommand command =
                new(sectionProfileData) { ModelId = this.ModelId };
            await createSectionProfileCommandHandler.ExecuteAsync(command);
        }
        else
        {
            PutSectionProfileCommand command =
                new()
                {
                    Id = this.sectionProfile.Id,
                    ModelId = this.ModelId,
                    Body = sectionProfileData
                };

            await putSectionProfileCommandHandler.ExecuteAsync(command);
        }
    }

    private static readonly int[] NullInt = [0];

    private Task<IEnumerable<int>> SectionProfileIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(
                NullInt.Concat(editorState.Value.CachedModelResponse.SectionProfiles.Keys)
            );
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            NullInt.Concat(
                editorState
                    .Value
                    .CachedModelResponse
                    .SectionProfiles
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

    public class SectionProfileModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public double? Area { get; set; }
        public double? StrongAxisMomentOfInertia { get; set; }
        public double? WeakAxisMomentOfInertia { get; set; }
        public double? PolarMomentOfInertia { get; set; }
        public double? StrongAxisShearArea { get; set; }
        public double? WeakAxisShearArea { get; set; }
        public AreaUnit AreaUnit { get; set; }
        public AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit { get; set; }
    }
}

[FeatureState]
public record SectionProfileObjectEditorState(bool IsLoading)
{
    public SectionProfileObjectEditorState()
        : this(false) { }
}

public record struct SectionProfileObjectEditorIsLoading(bool IsLoading);

public static class SectionProfileObjectEditorStateReducers
{
    [ReducerMethod]
    public static SectionProfileObjectEditorState Reducer(
        SectionProfileObjectEditorState state,
        SectionProfileObjectEditorIsLoading action
    ) => state with { IsLoading = action.IsLoading };
}
