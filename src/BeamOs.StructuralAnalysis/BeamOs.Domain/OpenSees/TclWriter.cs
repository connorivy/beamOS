using System.Text;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;

namespace BeamOs.Domain.OpenSees;

public class TclWriter
{
    public static string Write()
    {
        return null;
    }

    private readonly Dictionary<int, Guid> runtimeIdToDbDict = [];
    private readonly Dictionary<Guid, int> dbIdToRuntimeIdDict = [];

    private readonly UnitSettings unitSettings;

    public TclWriter(UnitSettings unitSettings)
    {
        this.unitSettings = unitSettings;
        this.document.AppendLine("wipe");
        this.document.AppendLine("model basic -ndm 3 -ndf 6");
        this.document.AppendLine("file mkdir Data");
    }

    private readonly StringBuilder document = new();

    private readonly HashSet<Guid> addedNodes = [];

    public void AddNode(Node node)
    {
        if (!this.addedNodes.Add(node.Id))
        {
            return;
        }
        int runtimeId = this.GetRuntimeId(node.Id);
        this.document.AppendLine(this.NodeLocationString(runtimeId, node.LocationPoint));
        this.document.AppendLine(NodeRestraintString(runtimeId, node.Restraint));
    }

    private string NodeLocationString(int id, Point point) =>
        $"node {id} {point.XCoordinate.As(this.unitSettings.LengthUnit)} {point.YCoordinate.As(this.unitSettings.LengthUnit)} {point.ZCoordinate.As(this.unitSettings.LengthUnit)}";

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

    public void AddMaterial(Material material)
    {
        double v = .3; // currently hardcoding poissons ratio
        int runtimeId = this.GetRuntimeId(material.Id);
        double e = material.ModulusOfElasticity.As(this.unitSettings.PressureUnit);
        double g = material.ModulusOfRigidity.As(this.unitSettings.PressureUnit);
        this.document.AppendLine(
            $"nDMaterial ElasticOrthotropic {runtimeId} {e} {e} {e} {v} {v} {v} {g} {g} {g}"
        );
    }

    private readonly HashSet<Guid> sections = [];

    public void AddSection(SectionProfile sectionProfile, Material material)
    {
        Guid combinedId = CombineGuids(sectionProfile.Id.Id, material.Id.Id);
        if (!this.sections.Add(combinedId))
        {
            return;
        }

        int runtimeId = this.GetRuntimeId(combinedId);
        this.document.AppendLine(
            $"section Elastic {runtimeId} {material.ModulusOfElasticity.As(this.unitSettings.PressureUnit)} {sectionProfile.Area.As(this.unitSettings.AreaUnit)} {sectionProfile.StrongAxisMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)} {sectionProfile.WeakAxisMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)} {material.ModulusOfRigidity.As(this.unitSettings.PressureUnit)} {sectionProfile.PolarMomentOfInertia.As(this.unitSettings.AreaMomentOfInertiaUnit)}"
        );
    }

    private static Guid transformationOffset = Guid.Parse("9f1ae054-1583-43f1-a362-6e653a8cd33f");

    public void AddElement(Element1D element1d)
    {
        int runtimeId = this.GetRuntimeId(element1d.Id);
        int startNodeId = this.GetRuntimeId(element1d.StartNodeId);
        int endNodeId = this.GetRuntimeId(element1d.EndNodeId);
        Guid combinedId = CombineGuids(element1d.SectionProfileId, element1d.MaterialId);
        int sectionId = this.GetRuntimeId(combinedId);

        var rotationMatrix = element1d.GetRotationMatrix();
        Guid transformationOffsetId = CombineGuids(element1d.Id, transformationOffset);

        int tranformId = this.GetRuntimeId(transformationOffsetId);
        this.document.AppendLine(
            $"geomTransf Linear {tranformId} {rotationMatrix[1, 0]} {rotationMatrix[1, 1]} {rotationMatrix[1, 2]}"
        );
        this.document.AppendLine(
            $"element {nameof(ElementTypes.elasticBeamColumn)} {runtimeId} {startNodeId} {endNodeId} {sectionId} {tranformId}"
        );
    }

    public void AddHydratedElement(Element1D element1d)
    {
        this.AddNode(element1d.StartNode);
        this.AddNode(element1d.EndNode);
        this.AddSection(element1d.SectionProfile, element1d.Material);
        this.AddElement(element1d);
    }

    public void AddPointLoads(IEnumerable<PointLoad> pointLoads)
    {
        this.document.AppendLine($"timeSeries {nameof(TimeSeriesType.Constant)} 1");
        this.document.AppendLine($"pattern {nameof(PatternType.Plain)} 1 1 {{");
        foreach (var pl in pointLoads)
        {
            this.AddPointLoad(pl);
        }
        this.document.AppendLine("}");
    }

    public void AddPointLoad(PointLoad pointLoad)
    {
        int runtimeId = this.GetRuntimeId(pointLoad.Id);
        int nodeId = this.GetRuntimeId(pointLoad.NodeId);

        this.document.AppendLine(
            $"load {nodeId} {pointLoad.Force.As(this.unitSettings.ForceUnit) * pointLoad.Direction.X} {pointLoad.Force.As(this.unitSettings.ForceUnit) * pointLoad.Direction.Y} {pointLoad.Force.As(this.unitSettings.ForceUnit) * pointLoad.Direction.Z} 0 0 0"
        );
    }

    public void DefineAnalysis()
    {
        this.document.AppendLine(
            "recorder Node -file Data/DFree.out -time -tcp 127.0.0.1 13000 -dof 1 2 3 disp"
        );
        this.document.AppendLine($"system {nameof(SystemType.BandGeneral)}");
        this.document.AppendLine($"numberer RCM");
        this.document.AppendLine($"constraints Transformation");
        this.document.AppendLine($"integrator LoadControl 1");
        this.document.AppendLine($"algorithm Linear");
        this.document.AppendLine($"analysis Static");
        this.document.AppendLine($"analyze 1");
    }

    private int GetRuntimeId(Guid id)
    {
        if (this.dbIdToRuntimeIdDict.TryGetValue(id, out var runtimeId))
        {
            return runtimeId;
        }

        runtimeId = this.dbIdToRuntimeIdDict.Count;
        this.dbIdToRuntimeIdDict[id] = runtimeId;
        return runtimeId;
    }

    private static Guid CombineGuids(params Guid[] guids)
    {
        byte[] totalBytes = new byte[16];
        foreach (var guid in guids)
        {
            var bytes = guid.ToByteArray();
            for (int i = 0; i < 16; i++)
            {
                totalBytes[i] += bytes[i];
            }
        }
        return new Guid(totalBytes);
    }

    public void Write(string filename)
    {
        File.WriteAllText(filename, this.document.ToString());
    }

    public override string ToString() => this.document.ToString();
}

public enum ElementTypes
{
    Undefined = 0,
    Truss,
    elasticBeamColumn,
    ModElasticBeam2d,
    GradientInelasticBeamColumn
}

public enum TimeSeriesType
{
    Undefined = 0,
    Constant
}

public enum PatternType
{
    Undefined = 0,
    Plain,
    UniformExcitation,
    MultipleSupport,
    DRM,
    H5DRM
}

public enum SystemType
{
    Undefined = 0,
    BandGeneral
}

//public enum OpenseesObjectTypes
