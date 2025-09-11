using System.Diagnostics;
using System.Text;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.OpenSees;

internal class TclWriter
{
    private readonly SortedSet<int> nodeIdsInOrder = [];
    private readonly SortedSet<int> element1dIdsInOrder = [];

    private readonly ModelSettings modelSettings;
    private readonly UnitSettings unitSettings;
    private readonly BeamOsToOpenSeesConverter beamOsToOpenSeesConverter;

    private readonly int displacementPort;
    private readonly int reactionPort;
    private readonly int elementForcesPort;

    public TclWriter(
        ModelSettings modelSettings,
        int displacementPort,
        int reactionPort,
        int elementForcesPort,
        UnitSettings? unitSettingsOverride = null
    )
    {
        this.modelSettings = modelSettings;
        this.unitSettings = unitSettingsOverride ?? modelSettings.UnitSettings;
        this.beamOsToOpenSeesConverter = new(modelSettings, unitSettingsOverride);

        this.document.AppendLine("wipe");
        this.document.AppendLine("model basic -ndm 3 -ndf 6");
        //this.document.AppendLine("file mkdir Data");
        this.displacementPort = displacementPort;
        this.reactionPort = reactionPort;
        this.elementForcesPort = elementForcesPort;
    }

    private readonly StringBuilder document = new();

    private readonly HashSet<int> addedNodes = [];

    public string? OutputFileWithPath { get; private set; }

    public void AddNode(Node node)
    {
        if (!this.addedNodes.Add(node.Id))
        {
            return;
        }
        this.nodeIdsInOrder.Add(node.Id);
        this.document.AppendLine(this.NodeLocationString(node.Id, node.LocationPoint));
        this.document.AppendLine(NodeRestraintString(node.Id, node.Restraint));
    }

    private string NodeLocationString(int id, Point point) =>
        $"node {id} {point.X.As(this.unitSettings.LengthUnit)} {point.Y.As(this.unitSettings.LengthUnit)} {point.Z.As(this.unitSettings.LengthUnit)}";

    private static string NodeRestraintString(int id, Restraint restraint)
    {
        static int ToSeesVal(bool canMove)
        {
            if (canMove)
            {
                return 0;
            }
            return 1;
        }
        return $"fix {id} {ToSeesVal(restraint.CanTranslateAlongX)} {ToSeesVal(restraint.CanTranslateAlongY)} {ToSeesVal(restraint.CanTranslateAlongZ)} {ToSeesVal(restraint.CanRotateAboutX)} {ToSeesVal(restraint.CanRotateAboutY)} {ToSeesVal(restraint.CanRotateAboutZ)}";
    }

    private readonly HashSet<int> sections = [];

    public void AddSection(SectionProfile sectionProfile, Material material)
    {
        int combinedId = GetSectionId(sectionProfile.Id, material.Id);
        if (!this.sections.Add(combinedId))
        {
            return;
        }

        this.document.AppendLine(
            $"section Elastic {combinedId} {material.ModulusOfElasticity.As(this.unitSettings.PressureUnit)} {sectionProfile.Area.As(this.unitSettings.AreaUnit)} {sectionProfile.StrongAxisMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)} {sectionProfile.WeakAxisMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)} {material.ModulusOfRigidity.As(this.unitSettings.PressureUnit)} {sectionProfile.PolarMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)}"
        );
    }

    public static int GetSectionId(SectionProfileId first, MaterialId second)
    {
        string combinedString = first.Id.ToString() + second.Id.ToString();
        return int.Parse(combinedString);
    }

    private readonly Dictionary<int, int> transformIds = [];

    public void AddElement(Element1d element1d)
    {
        this.element1dIdsInOrder.Add(element1d.Id);
        //int startNodeId = element1d.StartNodeId;
        //int endNodeId = element1d.EndNodeId;
        //int sectionId = GetSectionId(element1d.SectionProfileId, element1d.MaterialId);

        Debug.Assert(element1d.Material is not null);
        Debug.Assert(element1d.SectionProfile is not null);
        Debug.Assert(element1d.StartNode is not null);
        Debug.Assert(element1d.EndNode is not null);

        var rotationMatrix = element1d.GetRotationMatrix();
        // todo: if the strong axis of a section is different from the global Y axis,
        // then this would not be correct.
        // https://www.youtube.com/watch?v=1IMasmvlVwc
        var rotationX = rotationMatrix[1, 0];
        var rotationY = rotationMatrix[1, 1];
        var rotationZ = rotationMatrix[1, 2];
        int hash = GetTransformHash(rotationX, rotationY, rotationZ);
        if (!this.transformIds.TryGetValue(hash, out var transformId))
        {
            transformId = this.transformIds.Count;
            this.transformIds.Add(hash, transformId);
            this.document.AppendLine(
                $"geomTransf Linear {transformId} {rotationX} {rotationY} {rotationZ}"
            );
        }

        this.document.AppendLine(
            this.beamOsToOpenSeesConverter.Element1d(
                element1d,
                element1d.Material,
                element1d.SectionProfile.GetSectionProfile(),
                transformId
            )
        );

        //this.document.AppendLine(
        //    $"element {nameof(ElementTypes.ElasticTimoshenkoBeam)} {element1d.Id} {startNodeId} {endNodeId} {element1d.Material.ModulusOfElasticity.As(this.unitSettings.PressureUnit)} {element1d.Material.ModulusOfRigidity.As(this.unitSettings.PressureUnit)} {element1d.SectionProfile.Area.As(this.unitSettings.AreaUnit)} {element1d.SectionProfile.PolarMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)} {element1d.SectionProfile.StrongAxisMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)} {element1d.SectionProfile.WeakAxisMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)} {element1d.SectionProfile.StrongAxisShearArea.As(this.unitSettings.AreaUnit)} {element1d.SectionProfile.WeakAxisShearArea.As(this.unitSettings.AreaUnit)} {transformId}"
        //);
    }

    private static int GetTransformHash(double x, double y, double z)
    {
        int hash = 17;
        unchecked
        {
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            hash = hash * 23 + z.GetHashCode();
        }

        return hash;
    }

    public void AddHydratedElement(Element1d element1d)
    {
        Debug.Assert(element1d.Material is not null);
        Debug.Assert(element1d.SectionProfile is not null);
        Debug.Assert(element1d.StartNode is not null);
        Debug.Assert(element1d.EndNode is not null);
        Debug.Assert(element1d.InternalNodes is not null);

        foreach (
            var element in element1d.BreakBetweenInternalNodes(
                (_, _) => (this.element1dIdsInOrder.Count + 1) * -1
            )
        )
        {
            this.AddNode(element.StartNode.ToNode());
            this.AddNode(element.EndNode.ToNode());
            this.AddSection(element.SectionProfile.GetSectionProfile(), element.Material);
            this.AddElement(element);
        }
    }

    public void AddLoads(
        IEnumerable<PointLoad> pointLoads,
        IEnumerable<MomentLoad> momentLoads,
        LoadCombination combo
    )
    {
        this.document.AppendLine($"timeSeries {nameof(TimeSeriesType.Constant)} 1");
        this.document.AppendLine($"pattern {nameof(PatternType.Plain)} 1 1 {{");
        foreach (var pl in pointLoads)
        {
            this.AddPointLoad(pl, combo);
        }
        foreach (var load in momentLoads)
        {
            this.AddMomentLoad(load, combo);
        }
        this.document.AppendLine("}");
    }

    public void AddPointLoad(PointLoad pointLoad, LoadCombination combo)
    {
        int nodeId = pointLoad.NodeId;
        var x = pointLoad.GetScaledForce(
            CoordinateSystemDirection3D.AlongX,
            this.unitSettings.ForceUnit,
            combo
        );
        var y = pointLoad.GetScaledForce(
            CoordinateSystemDirection3D.AlongY,
            this.unitSettings.ForceUnit,
            combo
        );
        var z = pointLoad.GetScaledForce(
            CoordinateSystemDirection3D.AlongZ,
            this.unitSettings.ForceUnit,
            combo
        );

        this.document.AppendLine($"load {nodeId} {x} {y} {z} 0 0 0");
    }

    public void AddMomentLoad(MomentLoad momentLoad, LoadCombination combo)
    {
        int nodeId = momentLoad.NodeId;
        var x = momentLoad.GetScaledTorque(
            CoordinateSystemDirection3D.AboutX,
            this.unitSettings.TorqueUnit,
            combo
        );
        var y = momentLoad.GetScaledTorque(
            CoordinateSystemDirection3D.AboutY,
            this.unitSettings.TorqueUnit,
            combo
        );
        var z = momentLoad.GetScaledTorque(
            CoordinateSystemDirection3D.AboutZ,
            this.unitSettings.TorqueUnit,
            combo
        );

        this.document.AppendLine($"load {nodeId} 0 0 0 {x} {y} {z}");
    }

    public void DefineAnalysis()
    {
        this.document.AppendLine(
            $"recorder Node -time -tcp 127.0.0.1 {this.displacementPort} -dof 1 2 3 4 5 6 disp"
        );
        this.document.AppendLine(
            $"recorder Node -time -tcp 127.0.0.1 {this.reactionPort} -dof 1 2 3 4 5 6 reaction"
        );
        //this.document.AppendLine($"recorder Node -time -file disp.out -dof 1 2 3 4 5 6 disp");
        this.document.AppendLine(
            $"recorder Element -time -tcp 127.0.0.1 {this.elementForcesPort} -eleRange 0 {this.element1dIdsInOrder.Count} localForce"
        );

        //foreach (var el in this.element1dIdsInOrder)
        //{
        //}
        this.document.AppendLine(
            $"recorder Element -file Data/stiffness_matrix_{99}.out -time -eleRange 1 1 stiffness"
        );
        this.document.AppendLine(
            $"recorder Element -file Data/force_{99}.out -time -eleRange 1 {this.element1dIdsInOrder.Count} force"
        );

        // found a weird quirk where this call wouldn't output anything unless the eleRange was specified
        //this.document.AppendLine(
        //    $"recorder Element -time -file Data/forces.out -eleRange 0 {this.element1dIdsInOrder.Count} localForce"
        //);

        this.document.AppendLine($"system {nameof(SystemType.BandGeneral)}");
        this.document.AppendLine($"numberer RCM");
        this.document.AppendLine($"constraints Transformation");
        this.document.AppendLine($"integrator LoadControl 1");
        this.document.AppendLine($"algorithm {nameof(AlgorithmType.Newton)}");
        this.document.AppendLine($"analysis Static");
        this.document.AppendLine($"analyze 1");
        //this.document.AppendLine($"database File Data/aaaaaaDB");
        //this.document.AppendLine($"save 32");
    }

    public NodeId GetNodeIdFromOutputIndex(int index) => this.nodeIdsInOrder.ElementAt(index);

    public Element1dId GetElementIdFromOutputIndex(int index) =>
        this.element1dIdsInOrder.ElementAt(index);

    public void Write()
    {
        var tempPath = Path.GetTempPath();
        var fileName = Path.ChangeExtension(Guid.NewGuid().ToString(), ".tcl");
        this.OutputFileWithPath = Path.Combine(tempPath, fileName);
        File.WriteAllText(this.OutputFileWithPath, this.document.ToString());
    }

    public override string ToString() => this.document.ToString();
}

public enum ElementTypes
{
    Undefined = 0,
    Truss,
    elasticBeamColumn,
    ElasticTimoshenkoBeam,
    ModElasticBeam2d,
    GradientInelasticBeamColumn,
}

public enum TimeSeriesType
{
    Undefined = 0,
    Constant,
}

public enum PatternType
{
    Undefined = 0,
    Plain,
    UniformExcitation,
    MultipleSupport,
    DRM,
    H5DRM,
}

public enum SystemType
{
    Undefined = 0,
    BandGeneral,
    FullGeneral,
}

public enum AlgorithmType
{
    Undefined = 0,
    Linear,
    Newton,
}

public enum OpenseesObjectTypes
{
    Undefined = 0,
    Element1d,
    Node,
}

internal record TclWriterOptions();
