using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor;

public partial class NodeObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<NodeObjectEditorState> state,
    PutNodeCommandHandler putNodeCommandHandler
) : FluxorComponent
{
    private NodeModel node = new();

    private int nodeId
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

    private string nodeIdText => nodeId == 0 ? "New Node" : nodeId.ToString();

    private List<string> nodeIds;
    private static int[] nullInt = [0];
    private Task<IEnumerable<int>> NodeIds(string searchText, CancellationToken ct)
    {
        if (!int.TryParse(searchText, out int subInt))
        {
            return Task.FromResult(nullInt.Concat(editorState.Value.CachedModelResponse.Nodes.Keys));
        }

        // Get the number of digits in the subInt
        int subIntLength = GetNumberOfDigits(subInt);

        return Task.FromResult(nullInt.Concat(editorState.Value.CachedModelResponse.Nodes.Keys.Where(k => GetPrefix(k, subIntLength) == subInt)));
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

    [Parameter]
    public UnitSettingsContract UnitSettings { get; set; }

    [Parameter]
    public SelectedObject? SelectedObject { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.SubscribeToAction<PutObjectCommand<NodeResponse>>(command =>
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

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this.SelectedObject is not null && this.SelectedObject.TypeName == "Node")
        {
            UpdateFromNodeId(this.SelectedObject.Id);
        }
    }

    private void UpdateFromNodeId(int nodeId)
    {
        var response = editorState.Value.CachedModelResponse.Nodes[nodeId];
        this.UpdateFromNodeResponse(response);
    }

    private void UpdateFromNodeResponse(NodeResponse response)
    {
        var thisLengthUnit = UnitSettings.LengthUnit.MapToLengthUnit();
        var lengthUnit = response.LocationPoint.LengthUnit.MapToLengthUnit();
        node.Id = response.Id;
        node.ModelId = response.ModelId;
        node.LocationPoint.X = new Length(response.LocationPoint.X, lengthUnit).As(thisLengthUnit);
        node.LocationPoint.Y = new Length(response.LocationPoint.Y, lengthUnit).As(thisLengthUnit);
        node.LocationPoint.Z = new Length(response.LocationPoint.Z, lengthUnit).As(thisLengthUnit);
        node.LocationPoint.LengthUnit = response.LocationPoint.LengthUnit;
        node.Restraint.CanTranslateAlongX = response.Restraint.CanTranslateAlongX;
        node.Restraint.CanTranslateAlongY = response.Restraint.CanTranslateAlongY;
        node.Restraint.CanTranslateAlongZ = response.Restraint.CanTranslateAlongZ;
        node.Restraint.CanRotateAboutX = response.Restraint.CanRotateAboutX;
        node.Restraint.CanRotateAboutY = response.Restraint.CanRotateAboutY;
        node.Restraint.CanRotateAboutZ = response.Restraint.CanRotateAboutZ;
    }

    private void OnFocus(string fieldName, int fieldNum, Action<object> setValue)
    {
        dispatcher.Dispatch(
            new FieldSelected(
                new(fieldName, fieldNum, setValue)
            )
        );
    }

    private async Task Submit()
    {
        PutNodeCommand command = new() { Id = node.Id, ModelId = node.ModelId, Body = new NodeData() { LocationPoint = new(node.LocationPoint.X.Value, node.LocationPoint.Y.Value, node.LocationPoint.Z.Value, UnitSettings.LengthUnit), Restraint = new(node.Restraint.CanTranslateAlongX, node.Restraint.CanTranslateAlongY, node.Restraint.CanTranslateAlongZ, node.Restraint.CanRotateAboutX, node.Restraint.CanRotateAboutY, node.Restraint.CanRotateAboutZ) } };
        await putNodeCommandHandler.ExecuteAsync(command);
    }

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
    public NodeObjectEditorState() : this(false)
    {
    }
}

public record struct NodeObjectEditorIsLoading(bool IsLoading);

public static class NodeObjectEditorStateReducers
{
    [ReducerMethod]
    public static NodeObjectEditorState Reducer(NodeObjectEditorState state, NodeObjectEditorIsLoading action) => state with { IsLoading = action.IsLoading };

}
