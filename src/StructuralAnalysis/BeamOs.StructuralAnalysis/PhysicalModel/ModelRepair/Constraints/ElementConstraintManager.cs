using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Constraints;

public sealed class ElementConstraintManager(
    ModelProposalNodeStore nodeStore,
    ModelProposalElement1dStore element1dStore
)
{
    private readonly Dictionary<Element1dId, List<IElementConstraint>> elementConstraints = [];

    public void AddConstraints(
        Model model,
        ModelRepairOperationParameters modelRepairOperationParameters
    )
    {
        foreach (var element in model.Element1ds)
        {
            if (element is Element1d element1d)
            {
                this.AddConstraint(element1d, modelRepairOperationParameters);
            }
        }
    }

    public void AddConstraint(
        Element1d element,
        ModelRepairOperationParameters modelRepairOperationParameters
    )
    {
        var startNode = nodeStore[element.StartNodeId];
        var endNode = nodeStore[element.EndNodeId];
        var originalStartNodeLocation = startNode.GetLocationPoint(element1dStore, nodeStore);
        var originalEndNodeLocation = endNode.GetLocationPoint(element1dStore, nodeStore);

        if (
            Math.Abs((originalStartNodeLocation.X - originalEndNodeLocation.X).Meters)
            < ModelRepairRuleUtils.GetToleranceValue(
                AxisAlignmentToleranceLevel.Strict,
                modelRepairOperationParameters
            )
        )
        {
            this.AddConstraint(
                element.Id,
                new EqualCoordinateConstraint(CoordinateSystemDirection3D.AlongX)
            );
        }
        if (
            Math.Abs((originalStartNodeLocation.Y - originalEndNodeLocation.Y).Meters)
            < ModelRepairRuleUtils.GetToleranceValue(
                AxisAlignmentToleranceLevel.Strict,
                modelRepairOperationParameters
            )
        )
        {
            this.AddConstraint(
                element.Id,
                new EqualCoordinateConstraint(CoordinateSystemDirection3D.AlongY)
            );
        }
        if (
            Math.Abs((originalStartNodeLocation.Z - originalEndNodeLocation.Z).Meters)
            < ModelRepairRuleUtils.GetToleranceValue(
                AxisAlignmentToleranceLevel.Strict,
                modelRepairOperationParameters
            )
        )
        {
            this.AddConstraint(
                element.Id,
                new EqualCoordinateConstraint(CoordinateSystemDirection3D.AlongZ)
            );
        }
    }

    public void AddConstraint(Element1dId elementId, IElementConstraint constraint)
    {
        if (!this.elementConstraints.TryGetValue(elementId, out var list))
        {
            list = [];
            this.elementConstraints[elementId] = list;
        }
        list.Add(constraint);
    }

    public bool NodeMovementSatisfiesElementConstraints(
        Element1d element1d,
        NodeDefinition startNode,
        NodeDefinition endNode,
        ModelRepairOperationParameters modelRepairOperationParameters,
        Point? newStartNodeLocation,
        Point? newEndNodeLocation
    )
    {
        var originalStartNodeLocation = startNode.GetLocationPoint(element1dStore, nodeStore);
        var originalEndNodeLocation = endNode.GetLocationPoint(element1dStore, nodeStore);

        if (
            !this.elementConstraints.TryGetValue(element1d.Id, out var constraints)
            || constraints.Count == 0
        )
        {
            return true;
        }

        foreach (var constraint in constraints)
        {
            if (
                !constraint.IsSatisfied(
                    element1d,
                    modelRepairOperationParameters,
                    originalStartNodeLocation,
                    originalEndNodeLocation,
                    newStartNodeLocation,
                    newEndNodeLocation
                )
            )
            {
                return false;
            }
        }
        return true;
    }
}

public interface IElementConstraint
{
    public bool IsSatisfied(
        Element1d element1d,
        ModelRepairOperationParameters modelRepairOperationParameters,
        Point originalStartNodeLocation,
        Point originalEndNodeLocation,
        Point? newStartNodeLocation,
        Point? newEndNodeLocation
    );
}
