using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Features.SelectionInfo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor;

public partial class NodeObjectEditor(
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    IState<NodeObjectEditorState> state,
    PutNodeCommandHandler putNodeCommandHandler
) : FluxorComponent
{
    private NodeModel node = new();

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

        if (this.SelectedObject is not null)
        {
            var response = editorState.Value.CachedModelResponse.Nodes[this.SelectedObject.Id];
            UpdateFromNodeResponse(response);
        }
    }

    private void UpdateFromNodeResponse(NodeResponse response)
    {
        node.Id = response.Id;
        node.ModelId = response.ModelId;
        node.LocationPoint.X = response.LocationPoint.X;
        node.LocationPoint.Y = response.LocationPoint.Y;
        node.LocationPoint.Z = response.LocationPoint.Z;
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
        PutNodeCommand command = new() { Id = node.Id.Value, ModelId = node.ModelId, Body = new NodeData() { LocationPoint = new(node.LocationPoint.X.Value, node.LocationPoint.Y.Value, node.LocationPoint.Z.Value, UnitSettings.LengthUnit), Restraint = new(node.Restraint.CanTranslateAlongX, node.Restraint.CanTranslateAlongY, node.Restraint.CanTranslateAlongZ, node.Restraint.CanRotateAboutX, node.Restraint.CanRotateAboutY, node.Restraint.CanRotateAboutZ) } };
        await putNodeCommandHandler.ExecuteAsync(command);
    }

    public class NodeModel
    {
        public int? Id { get; set; }
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
