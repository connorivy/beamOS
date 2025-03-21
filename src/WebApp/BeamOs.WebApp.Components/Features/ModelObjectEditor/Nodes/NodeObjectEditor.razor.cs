using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Nodes;

public partial class NodeObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<NodeObjectEditorState> state,
    PutNodeCommandHandler putNodeCommandHandler,
    CreateNodeClientCommandHandler createNodeCommandHandler,
    DeleteNodeSimpleCommandHandler deleteNodeCommandHandler
) : FluxorComponent
{
    private readonly NodeModel node = new();

    private int NodeId
    {
        get => this.node.Id;
        set
        {
            if (value > 0)
            {
                this.UpdateFromNodeId(value);
            }
            this.node.Id = value;
        }
    }

    private const int ResultLimit = 50;

    private string NodeIdText => this.NodeId == 0 ? "New Node" : this.NodeId.ToString();

    [Parameter]
    public required UnitSettingsContract UnitSettings { get; set; }

    [Parameter]
    public required Guid ModelId { get; init; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        // putNodeCommandHandler.IsLoadingChanged += this.PutNodeCommandHandler_IsLoadingChanged;
        // createNodeCommandHandler.IsLoadingChanged += this.PutNodeCommandHandler_IsLoadingChanged;
        // deleteNodeCommandHandler.IsLoadingChanged += this.PutNodeCommandHandler_IsLoadingChanged;

        base.OnInitialized();
        this.SubscribeToAction<PutNodeClientCommand>(command =>
        {
            if (command.New is null)
            {
                return;
            }

            if (command.New.Id == this.SelectedObject?.Id)
            {
                this.UpdateFromNodeResponse(command.New);
            }
        });
    }

    // private void PutNodeCommandHandler_IsLoadingChanged(object? sender, bool e) =>
    //     dispatcher.Dispatch(new NodeObjectEditorIsLoading(e));

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this.SelectedObject is not null && this.SelectedObject.TypeName == "Node")
        {
            this.UpdateFromNodeId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromNodeId(int nodeId)
    {
        var response = editorState.Value.CachedModelResponse.Nodes[nodeId];
        this.UpdateFromNodeResponse(response);
    }

    private void UpdateFromNodeResponse(NodeResponse response)
    {
        var thisLengthUnit = this.UnitSettings.LengthUnit.MapToLengthUnit();
        var lengthUnit = response.LocationPoint.LengthUnit.MapToLengthUnit();
        this.node.Id = response.Id;
        this.node.ModelId = response.ModelId;
        this.node.LocationPoint.X = new Length(response.LocationPoint.X, lengthUnit).As(
            thisLengthUnit
        );
        this.node.LocationPoint.Y = new Length(response.LocationPoint.Y, lengthUnit).As(
            thisLengthUnit
        );
        this.node.LocationPoint.Z = new Length(response.LocationPoint.Z, lengthUnit).As(
            thisLengthUnit
        );
        this.node.LocationPoint.LengthUnit = response.LocationPoint.LengthUnit;
        this.node.Restraint.CanTranslateAlongX = response.Restraint.CanTranslateAlongX;
        this.node.Restraint.CanTranslateAlongY = response.Restraint.CanTranslateAlongY;
        this.node.Restraint.CanTranslateAlongZ = response.Restraint.CanTranslateAlongZ;
        this.node.Restraint.CanRotateAboutX = response.Restraint.CanRotateAboutX;
        this.node.Restraint.CanRotateAboutY = response.Restraint.CanRotateAboutY;
        this.node.Restraint.CanRotateAboutZ = response.Restraint.CanRotateAboutZ;
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(new FieldSelected(new(fieldName, fieldNum, setValue)));
    }

    private async Task Delete()
    {
        ModelEntityCommand command = new() { ModelId = this.ModelId, Id = this.node.Id };
        await deleteNodeCommandHandler.ExecuteAsync(command);
    }

    private async Task Submit()
    {
        NodeData nodeData =
            new()
            {
                LocationPoint = new(
                    this.node.LocationPoint.X.Value,
                    this.node.LocationPoint.Y.Value,
                    this.node.LocationPoint.Z.Value,
                    this.UnitSettings.LengthUnit
                ),
                Restraint = new(
                    this.node.Restraint.CanTranslateAlongX,
                    this.node.Restraint.CanTranslateAlongY,
                    this.node.Restraint.CanTranslateAlongZ,
                    this.node.Restraint.CanRotateAboutX,
                    this.node.Restraint.CanRotateAboutY,
                    this.node.Restraint.CanRotateAboutZ
                )
            };

        if (this.node.Id == 0)
        {
            CreateNodeClientCommand command = new(nodeData) { ModelId = this.ModelId };
            await createNodeCommandHandler.ExecuteAsync(command);
        }
        else
        {
            PutNodeCommand command =
                new()
                {
                    Id = this.node.Id,
                    ModelId = this.ModelId,
                    Body = nodeData
                };

            await putNodeCommandHandler.ExecuteAsync(command);
        }
    }

    private static readonly int[] NullInt = [0];

    private Task<IEnumerable<int>> NodeIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(
                NullInt.Concat(editorState.Value.CachedModelResponse.Nodes.Keys)
            );
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(
            NullInt.Concat(
                editorState
                    .Value
                    .CachedModelResponse
                    .Nodes
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
    //     putNodeCommandHandler.IsLoadingChanged -= this.PutNodeCommandHandler_IsLoadingChanged;
    //     createNodeCommandHandler.IsLoadingChanged -= this.PutNodeCommandHandler_IsLoadingChanged;
    //     deleteNodeCommandHandler.IsLoadingChanged -= this.PutNodeCommandHandler_IsLoadingChanged;
    //     return base.DisposeAsyncCore(disposing);
    // }

    public class NodeModel
    {
        public int Id { get; set; }
        public Guid ModelId { get; set; }
        public PointModel LocationPoint { get; set; } = new();
        public RestraintModel Restraint { get; set; } = new();
    }

    public class PointModel
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public LengthUnitContract LengthUnit { get; set; }
    }

    public class RestraintModel
    {
        public bool CanTranslateAlongX { get; set; }
        public bool CanTranslateAlongY { get; set; }
        public bool CanTranslateAlongZ { get; set; }
        public bool CanRotateAboutX { get; set; }
        public bool CanRotateAboutY { get; set; }
        public bool CanRotateAboutZ { get; set; }
    }
}

[FeatureState]
public record NodeObjectEditorState(bool IsLoading)
{
    public NodeObjectEditorState()
        : this(false) { }
}

public record struct NodeObjectEditorIsLoading(bool IsLoading);

public static class NodeObjectEditorStateReducers
{
    [ReducerMethod]
    public static NodeObjectEditorState Reducer(
        NodeObjectEditorState state,
        NodeObjectEditorIsLoading action
    ) => state with { IsLoading = action.IsLoading };
}
