using System.Text;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.CodeGen.TestModelBuilderGenerator.Extensions;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;

namespace BeamOs.StructuralAnalysis.CsSdk;

public sealed class BeamOsDynamicModelBuilder(
    string guidString,
    PhysicalModelSettings physicalModelSettings,
    string name,
    string description
) : BeamOsModelBuilder
{
    public override string Name => name;
    public override string Description => description;
    public override PhysicalModelSettings Settings => physicalModelSettings;
    public override string GuidString => guidString;

    private readonly List<PutNodeRequest> nodes = [];

    public UnitSettingsContract UnitSettings => this.Settings.UnitSettings;

    public BeamOsDynamicModelBuilder(
        string guidString,
        PhysicalModelSettings physicalModelSettings,
        string name,
        string description,
        BeamOsModelBuilderDto beamOsModelBuilderDto
    )
        : this(guidString, physicalModelSettings, name, description)
    {
        this.nodes = beamOsModelBuilderDto.Nodes.ToList();
        this.materials = beamOsModelBuilderDto.Materials.ToList();
        this.element1ds = beamOsModelBuilderDto.Element1ds.ToList();
        this.pointLoads = beamOsModelBuilderDto.PointLoads.ToList();
        this.momentLoads = beamOsModelBuilderDto.MomentLoads.ToList();
        this.sectionProfiles = beamOsModelBuilderDto.SectionProfiles.ToList();
    }

    public void AddNodes(params Span<PutNodeRequest> nodes) => this.nodes.AddRange(nodes);

    public override IEnumerable<PutNodeRequest> NodeRequests() => this.nodes.AsReadOnly();

    private readonly List<PutElement1dRequest> element1ds = [];

    public void AddElement1ds(params Span<PutElement1dRequest> els) =>
        this.element1ds.AddRange(els);

    public override IEnumerable<PutElement1dRequest> Element1dRequests() =>
        this.element1ds.AsReadOnly();

    private readonly List<PutMaterialRequest> materials = [];

    public void AddMaterials(params Span<PutMaterialRequest> materials) =>
        this.materials.AddRange(materials);

    public override IEnumerable<PutMaterialRequest> MaterialRequests() =>
        this.materials.AsReadOnly();

    private readonly List<PutPointLoadRequest> pointLoads = [];

    public void AddPointLoads(params Span<PutPointLoadRequest> pointLoads) =>
        this.pointLoads.AddRange(pointLoads);

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests() =>
        this.pointLoads.AsReadOnly();

    private readonly List<PutMomentLoadRequest> momentLoads = [];

    public void AddMomentLoads(params Span<PutMomentLoadRequest> momentLoads) =>
        this.momentLoads.AddRange(momentLoads);

    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests() =>
        this.momentLoads.AsReadOnly();

    private readonly List<PutSectionProfileRequest> sectionProfiles = [];

    public void AddSectionProfiles(params Span<PutSectionProfileRequest> sectionProfiles) =>
        this.sectionProfiles.AddRange(sectionProfiles);

    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests() =>
        this.sectionProfiles.AsReadOnly();

    //public static async Task<BeamOsDynamicModelBuilder> CreateFromModel(ModelId modelId, IStructuralAnalysisApiClientV1 apiClient)
    //{
    //    var model = await apiClient.GetModelAsync(modelId);
    //    BeamOsDynamicModelBuilder modelBuilder = new(modelId.Id.ToString(), model.Value.Settings, model.Value.Name, model.Value.Description);
    //}

    public void GenerateStaticModelClass(string outputDir, string? baseClass = null)
    {
        var sb = new StringBuilder();

        string namespac =
            "BeamOs.Tests.Common"
            + outputDir.Split("BeamOs.Tests.Common")[1].Replace("\\", ".").Replace("/", ".");
        string className = this.Name.Replace(" ", "");

        sb.AppendLine(
            $@"
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.CsSdk;
using static BeamOs.StructuralAnalysis.Contracts.Common.AngleUnitContract;
using static BeamOs.StructuralAnalysis.Contracts.Common.LengthUnitContract;

namespace {namespac};"
        );
        sb.AppendLine();
        sb.AppendLine(
            $"public partial class {className} : {baseClass ?? nameof(BeamOsModelBuilder)}"
        );
        sb.AppendLine("{");
        sb.AppendLine($"    public override string Name => nameof({className});");
        sb.AppendLine($"    public override string Description => \"{this.Description}\";");
        sb.AppendLine(
            "    public override PhysicalModelSettings Settings => new PhysicalModelSettings"
        );
        sb.AppendLine("    (");
        sb.AppendLine("        unitSettings: new UnitSettingsContract");
        sb.AppendLine("        {");
        sb.AppendLine(
            $"            LengthUnit = LengthUnitContract.{this.Settings.UnitSettings.LengthUnit},"
        );
        sb.AppendLine(
            $"            ForceUnit = ForceUnitContract.{this.Settings.UnitSettings.ForceUnit},"
        );
        sb.AppendLine(
            $"            AngleUnit = AngleUnitContract.{this.Settings.UnitSettings.AngleUnit}"
        );
        sb.AppendLine("        },");
        sb.AppendLine("        analysisSettings: new AnalysisSettingsContract");
        sb.AppendLine("        {");
        sb.AppendLine(
            $"            Element1DAnalysisType = Element1dAnalysisType.{this.Settings.AnalysisSettings.Element1DAnalysisType}"
        );
        sb.AppendLine("        },");
        sb.AppendLine($"        yAxisUp: {this.Settings.YAxisUp.ToString().ToLower()}");
        sb.AppendLine("    );");
        sb.AppendLine($"    public override string GuidString => \"{this.GuidString}\";");
        sb.AppendLine();
        sb.AppendLine("    public override IEnumerable<PutNodeRequest> NodeRequests()");
        sb.AppendLine("    {");
        foreach (var node in this.NodeRequests())
        {
            sb.AppendLine();
            sb.Append($"yield return new");
            sb.Append($"(");
            sb.Append($"{node.Id},");
            sb.Append(
                $"new({node.LocationPoint.X}, {node.LocationPoint.Y}, {node.LocationPoint.Z},{node.LocationPoint.LengthUnit}),"
            );
            sb.Append(
                $"new({node.Restraint.CanTranslateAlongX.ToString().ToLower()}, {node.Restraint.CanTranslateAlongY.ToString().ToLower()}, {node.Restraint.CanTranslateAlongZ.ToString().ToLower()}, {node.Restraint.CanRotateAboutX.ToString().ToLower()}, {node.Restraint.CanRotateAboutY.ToString().ToLower()}, {node.Restraint.CanRotateAboutZ.ToString().ToLower()})"
            );
            //sb.AppendLine($"            Metadata = {(node.Metadata != null ? $"new Dictionary<string, string> {{ /* Initialize metadata here */ }}" : "null")}");
            sb.Append($");");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public override IEnumerable<PutMaterialRequest> MaterialRequests()");
        sb.AppendLine("    {");
        foreach (var material in this.MaterialRequests())
        {
            sb.AppendLine($"        yield return new PutMaterialRequest");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            Id = {material.Id},");
            sb.AppendLine(
                $"            ModulusOfElasticity = new PressureContract({material.ModulusOfElasticity.Value}, PressureUnitContract.{material.ModulusOfElasticity.Unit}),"
            );
            sb.AppendLine(
                $"            ModulusOfRigidity = new PressureContract({material.ModulusOfRigidity.Value}, PressureUnitContract.{material.ModulusOfRigidity.Unit})"
            );
            sb.AppendLine($"        }};");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine(
            "    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests()"
        );
        sb.AppendLine("    {");
        foreach (var sectionProfile in this.SectionProfileRequests())
        {
            sb.AppendLine($"        yield return new PutSectionProfileRequest");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            Id = {sectionProfile.Id},");
            sb.AppendLine(
                $"            Area = new AreaContract({sectionProfile.Area.Value}, AreaUnitContract.{sectionProfile.Area.Unit}),"
            );
            sb.AppendLine(
                $"            StrongAxisMomentOfInertia = new AreaMomentOfInertiaContract({sectionProfile.StrongAxisMomentOfInertia.Value}, AreaMomentOfInertiaUnitContract.{sectionProfile.StrongAxisMomentOfInertia.Unit}),"
            );
            sb.AppendLine(
                $"            WeakAxisMomentOfInertia = new AreaMomentOfInertiaContract({sectionProfile.WeakAxisMomentOfInertia.Value}, AreaMomentOfInertiaUnitContract.{sectionProfile.WeakAxisMomentOfInertia.Unit}),"
            );
            sb.AppendLine(
                $"            PolarMomentOfInertia = new AreaMomentOfInertiaContract({sectionProfile.PolarMomentOfInertia.Value}, AreaMomentOfInertiaUnitContract.{sectionProfile.PolarMomentOfInertia.Unit}),"
            );
            sb.AppendLine(
                $"            StrongAxisShearArea = new AreaContract({sectionProfile.StrongAxisShearArea.Value}, AreaUnitContract.{sectionProfile.StrongAxisShearArea.Unit}),"
            );
            sb.AppendLine(
                $"            WeakAxisShearArea = new AreaContract({sectionProfile.WeakAxisShearArea.Value}, AreaUnitContract.{sectionProfile.WeakAxisShearArea.Unit})"
            );
            sb.AppendLine($"        }};");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public override IEnumerable<PutElement1dRequest> Element1dRequests()");
        sb.AppendLine("    {");
        foreach (var element in this.Element1dRequests())
        {
            sb.AppendLine();
            sb.Append($"yield return new(");
            sb.Append($"{element.Id},");
            sb.Append($"{element.StartNodeId},");
            sb.Append($"{element.EndNodeId},");
            sb.Append($"{element.MaterialId},");
            sb.Append($"{element.SectionProfileId},");
            sb.Append(
                $"new({element.SectionProfileRotation.Value.Value}, {element.SectionProfileRotation.Value.Unit})"
            );
            //sb.AppendLine($"            Metadata = {(element.Metadata != null ? $"new Dictionary<string, string> {{ /* Initialize metadata here */ }}" : "null")}");
            sb.Append($");");
        }
        sb.AppendLine("    }");
        sb.AppendLine();

        if (this.pointLoads.Count > 0)
        {
            sb.AppendLine(
                "    public override IEnumerable<PutPointLoadRequest> PointLoadRequests()"
            );
            sb.AppendLine("    {");
            foreach (var pointLoad in this.PointLoadRequests())
            {
                sb.AppendLine();
                sb.Append($"yield return new PutPointLoadRequest");
                sb.Append($"{{");
                sb.Append($"Id = {pointLoad.Id},");
                sb.Append($"NodeId = {pointLoad.NodeId},");
                sb.Append(
                    $"Force = new({pointLoad.Force.Value}, ForceUnitContract.{pointLoad.Force.Unit}),"
                );
                sb.Append(
                    $"Direction = new({pointLoad.Direction.X}, {pointLoad.Direction.Y}, {pointLoad.Direction.Z})"
                );
                sb.Append($"}};");
            }
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        if (this.momentLoads.Count > 0)
        {
            sb.AppendLine(
                "    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests()"
            );
            sb.AppendLine("    {");
            foreach (var momentLoad in this.MomentLoadRequests())
            {
                sb.AppendLine($"        yield return new PutMomentLoadRequest");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            Id = {momentLoad.Id},");
                sb.AppendLine($"            NodeId = {momentLoad.NodeId},");
                sb.AppendLine(
                    $"            Torque = new TorqueContract({momentLoad.Torque.Value}, TorqueUnitContract.{momentLoad.Torque.Unit}),"
                );
                sb.AppendLine(
                    $"            AxisDirection = new Vector3({momentLoad.AxisDirection.X}, {momentLoad.AxisDirection.Y}, {momentLoad.AxisDirection.Z})"
                );
                sb.AppendLine($"        }};");
            }
            sb.AppendLine("    }");
        }
        sb.AppendLine("}");

        File.WriteAllText(Path.Combine(outputDir, className + ".g.cs"), sb.ToString());
    }

    public void WriteToPyniteFile(string outputDir)
    {
        StringBuilder sb = new();

        string className = this.Name.Replace(" ", "");

        sb.AppendLine(
            @"
from Pynite import FEModel3D
import time

model = FEModel3D()"
        );

        foreach (var node in this.NodeRequests())
        {
            sb.AppendLine(
                $"model.add_node('N{node.Id}', {node.LocationPoint.X.Convert(node.LocationPoint.LengthUnit, this.Settings.UnitSettings.LengthUnit)}, {node.LocationPoint.Y.Convert(node.LocationPoint.LengthUnit, this.Settings.UnitSettings.LengthUnit)}, {node.LocationPoint.Z.Convert(node.LocationPoint.LengthUnit, this.Settings.UnitSettings.LengthUnit)})"
            );
            sb.AppendLine(
                $"model.def_support('N{node.Id}', {node.Restraint.CanTranslateAlongX.ToString()}, {node.Restraint.CanTranslateAlongY.ToString()}, {node.Restraint.CanTranslateAlongZ.ToString()}, {node.Restraint.CanRotateAboutX.ToString()}, {node.Restraint.CanRotateAboutY.ToString()}, {node.Restraint.CanRotateAboutZ.ToString()})"
            );
        }

        sb.AppendLine();
        foreach (var material in this.MaterialRequests())
        {
            sb.AppendLine(
                @$"
nu = 0.3  # Poisson's ratio
rho = 0.490/12**3  # Density (kci)
model.add_material('M{material.Id}', {material.ModulusOfElasticity.As(this.UnitSettings.PressureUnit)}, {material.ModulusOfRigidity.As(this.UnitSettings.PressureUnit)}, nu, rho)"
            );
        }
        sb.AppendLine();
        foreach (var sectionProfile in this.SectionProfileRequests())
        {
            sb.AppendLine(
                $"model.add_section('S{sectionProfile.Id}', {sectionProfile.Area.As(this.Settings.UnitSettings.AreaUnit)}, {sectionProfile.WeakAxisMomentOfInertia.As(this.UnitSettings.AreaMomentOfInertiaUnit)}, {sectionProfile.StrongAxisMomentOfInertia.As(this.UnitSettings.AreaMomentOfInertiaUnit)}, {sectionProfile.PolarMomentOfInertia.As(this.UnitSettings.AreaMomentOfInertiaUnit)})"
            );
        }
        sb.AppendLine();
        foreach (var element in this.Element1dRequests())
        {
            sb.AppendLine(
                $"model.add_member('El{element.Id}', 'N{element.StartNodeId}', 'N{element.EndNodeId}', 'M{element.MaterialId}', 'S{element.SectionProfileId}')"
            );
        }
        sb.AppendLine();

        foreach (var pointLoad in this.PointLoadRequests())
        {
            var (pyniteDir, forceMult) = BeamOsDirectionToPyniteStringAndForceMultiplier(
                pointLoad.Direction
            );
            sb.AppendLine(
                $"model.add_node_load('N{pointLoad.NodeId}', 'F{pyniteDir}', {forceMult * pointLoad.Force.As(this.UnitSettings.ForceUnit)}, case='D')"
            );
        }
        sb.AppendLine();

        foreach (var momentLoad in this.MomentLoadRequests())
        {
            var (pyniteDir, forceMult) = BeamOsDirectionToPyniteStringAndForceMultiplier(
                momentLoad.AxisDirection
            );
            sb.AppendLine(
                $"model.add_node_load('N{momentLoad.NodeId}', 'M{pyniteDir}', {forceMult * momentLoad.Torque.As(this.UnitSettings.TorqueUnit)}, case='D')"
            );
        }

        sb.AppendLine(
            @"
model.add_load_combo('1.0D', factors={'D':1.0})

start_time = time.time()

model.analyze()

end_time = time.time()

execution_time = end_time - start_time

print(f""Execution time: {execution_time} seconds"")
"
        );

        File.WriteAllText(Path.Combine(outputDir, className + ".g.py"), sb.ToString());
    }

    private static (string, int) BeamOsDirectionToPyniteStringAndForceMultiplier(Vector3 direction)
    {
        if (Math.Abs(direction.X - 1) < .001)
        {
            return ("X", 1);
        }
        if (Math.Abs(direction.Y - 1) < .001)
        {
            return ("Y", 1);
        }
        if (Math.Abs(direction.Z - 1) < .001)
        {
            return ("Z", 1);
        }
        if (Math.Abs(direction.X + 1) < .001)
        {
            return ("X", -1);
        }
        if (Math.Abs(direction.Y + 1) < .001)
        {
            return ("Y", -1);
        }
        if (Math.Abs(direction.Z + 1) < .001)
        {
            return ("Z", -1);
        }

        throw new NotImplementedException(
            $"Translation from {direction} to pynite has not been implemented"
        );
    }
}
