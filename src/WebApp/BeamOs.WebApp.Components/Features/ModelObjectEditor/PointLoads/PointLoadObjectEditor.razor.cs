using System.Diagnostics.CodeAnalysis;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.PointLoads;

public partial class PointLoadObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<PointLoadObjectEditorState> state,
    PutPointLoadSimpleCommandHandler putPointLoadCommandHandler,
    CreatePointLoadClientCommandHandler createPointLoadCommandHandler,
    DeletePointLoadSimpleCommandHandler deletePointLoadCommandHandler
) : FluxorComponent
{
    private readonly PointLoadModel pointLoad = new();

    private int PointLoadId
    {
        get => this.pointLoad.Id;
        set
        {
            if (value > 0)
            {
                this.UpdateFromPointLoadId(value);
            }
            this.pointLoad.Id = value;
        }
    }

    private LoadCase? LoadCase
    {
        get =>
            editorState.Value.CachedModelResponse.LoadCases.GetValueOrDefault(
                this.pointLoad.LoadCaseId
            );
        set => this.pointLoad.LoadCaseId = value.Id;
    }

    private const int ResultLimit = 50;

    private string PointLoadIdText =>
        this.PointLoadId == 0 ? "New Point Load" : this.PointLoadId.ToString();

    [Parameter]
    public required UnitSettingsContract UnitSettings { get; set; }

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.SubscribeToAction<PutPointLoadClientCommand>(command =>
        {
            if (command.New is null)
            {
                return;
            }

            if (command.New.Id == this.SelectedObject?.Id)
            {
                this.UpdateFromPointLoadResponse(command.New);
            }
        });
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this.SelectedObject is not null && this.SelectedObject.TypeName == "PointLoad")
        {
            this.UpdateFromPointLoadId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromPointLoadId(int pointLoadId)
    {
        var response = editorState.Value.CachedModelResponse.PointLoads[pointLoadId];
        this.UpdateFromPointLoadResponse(response);
    }

    private void UpdateFromPointLoadResponse(PointLoadResponse response)
    {
        var thisForceUnit = this.UnitSettings.ForceUnit.MapToForceUnit();
        var forceUnit = response.Force.Unit.MapToForceUnit();

        this.pointLoad.Id = response.Id;
        this.pointLoad.ModelId = response.ModelId;
        this.pointLoad.LoadCaseId = response.LoadCaseId;
        this.pointLoad.NodeId = response.NodeId;
        this.pointLoad.Direction.X = response.Direction.X;
        this.pointLoad.Direction.Y = response.Direction.Y;
        this.pointLoad.Direction.Z = response.Direction.Z;

        this.pointLoad.Force = new Force(response.Force.Value, forceUnit).As(thisForceUnit);
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(new FieldSelected(new(fieldName, fieldNum, setValue)));
    }

    private async Task Delete()
    {
        ModelEntityCommand command = new() { ModelId = this.ModelId, Id = this.pointLoad.Id };
        await deletePointLoadCommandHandler.ExecuteAsync(command);
    }

    private async Task Submit()
    {
        var pointLoadData = new PointLoadData()
        {
            NodeId = this.pointLoad.NodeId,
            LoadCaseId = this.pointLoad.LoadCaseId,
            Force = new(this.pointLoad.Force.Value, this.UnitSettings.ForceUnit),
            Direction = new(
                this.pointLoad.Direction.X.Value,
                this.pointLoad.Direction.Y.Value,
                this.pointLoad.Direction.Z.Value
            ),
        };

        if (this.pointLoad.Id == 0)
        {
            CreatePointLoadClientCommand command = new(pointLoadData) { ModelId = this.ModelId };
            await createPointLoadCommandHandler.ExecuteAsync(command);
        }
        else
        {
            PutPointLoadCommand command = new()
            {
                Id = this.pointLoad.Id,
                ModelId = this.ModelId,
                Body = pointLoadData,
            };

            await putPointLoadCommandHandler.ExecuteAsync(command);
        }
    }

    private Task<IEnumerable<int>> PointLoadIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(
                NullInt.Concat(editorState.Value.CachedModelResponse.PointLoads.Keys)
            );
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            NullInt.Concat(
                editorState.Value.CachedModelResponse.PointLoads.Keys.Where(k =>
                    GetPrefix(k, subIntLength) == subInt
                )
            )
        );
    }

    private Task<IEnumerable<int>> NodeIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(editorState.Value.CachedModelResponse.Nodes.Keys);
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            editorState.Value.CachedModelResponse.Nodes.Keys.Where(k =>
                GetPrefix(k, subIntLength) == subInt
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

    private static readonly int[] NullInt = [0];

    public class PointLoadModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public int LoadCaseId { get; set; }
        public int NodeId { get; set; }
        public double? Force { get; set; }
        public Vector3 Direction { get; set; } = new(0, 0, 0);
    }

    public record Vector3
    {
        public required double? X { get; set; }
        public required double? Y { get; set; }
        public required double? Z { get; set; }

        public Vector3() { }

        [SetsRequiredMembers]
        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}

[FeatureState]
public record PointLoadObjectEditorState(bool IsLoading)
{
    public PointLoadObjectEditorState()
        : this(false) { }
}

public record struct PointLoadObjectEditorIsLoading(bool IsLoading);

public static class PointLoadObjectEditorStateReducers
{
    [ReducerMethod]
    public static PointLoadObjectEditorState Reducer(
        PointLoadObjectEditorState state,
        PointLoadObjectEditorIsLoading action
    ) => state with { IsLoading = action.IsLoading };
}
