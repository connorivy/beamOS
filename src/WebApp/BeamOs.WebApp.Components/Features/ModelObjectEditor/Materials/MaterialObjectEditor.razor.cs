using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Materials;

public partial class MaterialObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<MaterialObjectEditorState> state,
    PutMaterialSimpleCommandHandler putMaterialCommandHandler,
    CreateMaterialClientCommandHandler createMaterialCommandHandler,
    DeleteMaterialSimpleCommandHandler deleteMaterialCommandHandler
) : FluxorComponent
{
    private readonly MaterialModel material = new();

    private int MaterialId
    {
        get => this.material.Id;
        set
        {
            if (value > 0)
            {
                this.UpdateFromMaterialId(value);
            }
            this.material.Id = value;
        }
    }

    private const int ResultLimit = 50;

    private string MaterialIdText =>
        this.MaterialId == 0 ? "New material" : this.MaterialId.ToString();

    [Parameter]
    public required UnitSettingsContract UnitSettings { get; set; }

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        // putMaterialCommandHandler.IsLoadingChanged += this.PutMaterialCommandHandler_IsLoadingChanged;
        // createMaterialCommandHandler.IsLoadingChanged += this.PutMaterialCommandHandler_IsLoadingChanged;
        // deleteMaterialCommandHandler.IsLoadingChanged += this.PutMaterialCommandHandler_IsLoadingChanged;

        base.OnInitialized();
        // this.SubscribeToAction<PutMaterialClientCommand>(command =>
        // {
        //     if (command.New is null)
        //     {
        //         return;
        //     }

        //     if (command.New.Id == this.SelectedObject?.Id)
        //     {
        //         this.UpdateFromMaterialResponse(command.New);
        //     }
        // });
    }

    // private void PutMaterialCommandHandler_IsLoadingChanged(object? sender, bool e) =>
    //     dispatcher.Dispatch(new MaterialObjectEditorIsLoading(e));

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (
            this.SelectedObject is not null
            && this.SelectedObject.ObjectType == BeamOsObjectType.Material
        )
        {
            this.UpdateFromMaterialId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromMaterialId(int materialId)
    {
        var response = editorState.Value.CachedModelResponse.Materials[materialId];
        this.UpdateFromMaterialResponse(response);
    }

    private void UpdateFromMaterialResponse(MaterialResponse response)
    {
        var pressureUnit = response.PressureUnit.MapToPressureUnit();
        var thisPressureUnit = this.UnitSettings.PressureUnit.MapToPressureUnit();
        this.material.Id = response.Id;
        this.material.ModelId = response.ModelId;
        this.material.ModulusOfElasticity = new Pressure(
            response.ModulusOfElasticity,
            pressureUnit
        ).As(thisPressureUnit);
        this.material.ModulusOfRigidity = new Pressure(response.ModulusOfRigidity, pressureUnit).As(
            thisPressureUnit
        );
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(new FieldSelected(new(fieldName, fieldNum, setValue)));
    }

    private async Task Delete()
    {
        ModelEntityCommand command = new() { ModelId = this.ModelId, Id = this.material.Id };
        await deleteMaterialCommandHandler.ExecuteAsync(command);
    }

    private async Task Submit()
    {
        MaterialData data = new()
        {
            PressureUnit = this.UnitSettings.PressureUnit,
            ModulusOfElasticity = this.material.ModulusOfElasticity.Value,
            ModulusOfRigidity = this.material.ModulusOfRigidity.Value,
        };

        if (this.material.Id == 0)
        {
            await createMaterialCommandHandler.ExecuteAsync(new(data) { ModelId = this.ModelId });
        }
        else
        {
            PutMaterialCommand command = new()
            {
                Id = this.material.Id,
                ModelId = this.ModelId,
                Body = data,
            };

            await putMaterialCommandHandler.ExecuteAsync(command);
        }
    }

    private static readonly int[] NullInt = [0];

    private Task<IEnumerable<int>> MaterialIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(
                NullInt.Concat(editorState.Value.CachedModelResponse.Materials.Keys)
            );
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            NullInt.Concat(
                editorState.Value.CachedModelResponse.Materials.Keys.Where(k =>
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

    // protected override ValueTask DisposeAsyncCore(bool disposing)
    // {
    //     putMaterialCommandHandler.IsLoadingChanged -= this.PutMaterialCommandHandler_IsLoadingChanged;
    //     createMaterialCommandHandler.IsLoadingChanged -= this.PutMaterialCommandHandler_IsLoadingChanged;
    //     deleteMaterialCommandHandler.IsLoadingChanged -= this.PutMaterialCommandHandler_IsLoadingChanged;
    //     return base.DisposeAsyncCore(disposing);
    // }

    public class MaterialModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public double? ModulusOfElasticity { get; set; }
        public double? ModulusOfRigidity { get; set; }
    }
}

[FeatureState]
public record MaterialObjectEditorState(bool IsLoading)
{
    public MaterialObjectEditorState()
        : this(false) { }
}

public record struct MaterialObjectEditorIsLoading(bool IsLoading);

public static class MaterialObjectEditorStateReducers
{
    [ReducerMethod]
    public static MaterialObjectEditorState Reducer(
        MaterialObjectEditorState state,
        MaterialObjectEditorIsLoading action
    ) => state with { IsLoading = action.IsLoading };
}
