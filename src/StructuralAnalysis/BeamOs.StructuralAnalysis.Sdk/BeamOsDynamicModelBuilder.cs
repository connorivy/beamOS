using System.Text;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk.Extensions;

namespace BeamOs.StructuralAnalysis.Sdk;

public sealed class BeamOsDynamicModelBuilder(
    string guidString,
    ModelSettings physicalModelSettings,
    string name,
    string description
) : BeamOsModelBuilder
{
    public override string Name => name;
    public override string Description => description;
    public override ModelSettings Settings => physicalModelSettings;
    public override string GuidString => guidString;

    private readonly List<PutNodeRequest> nodes = [];

    public UnitSettings UnitSettings => this.Settings.UnitSettings;

    public BeamOsDynamicModelBuilder(
        string guidString,
        ModelSettings physicalModelSettings,
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

    public void AddNode(int id, double x, double y, double z, Restraint? restraint = null) =>
        this.AddNodes(
            new PutNodeRequest()
            {
                Id = id,
                LocationPoint = new(x, y, z, this.UnitSettings.LengthUnit),
                Restraint = restraint ?? Restraint.Free,
            }
        );

    public void AddNodes(params Span<PutNodeRequest> nodes) => this.nodes.AddRange(nodes);

    public override IEnumerable<PutNodeRequest> NodeRequests() => this.nodes.AsReadOnly();

    private readonly List<PutElement1dRequest> element1ds = [];

    public void AddElement1d(
        int id,
        int startNodeId,
        int endNodeId,
        int materialId,
        int sectionProfileId,
        AngleContract? sectionProfileRotation = null
    ) =>
        this.AddElement1ds(
            new PutElement1dRequest(
                id,
                startNodeId,
                endNodeId,
                materialId,
                sectionProfileId,
                sectionProfileRotation ?? new(0, AngleUnitContract.Radian)
            )
        );

    public void AddElement1ds(params Span<PutElement1dRequest> els) =>
        this.element1ds.AddRange(els);

    public override IEnumerable<PutElement1dRequest> Element1dRequests() =>
        this.element1ds.AsReadOnly();

    private readonly List<PutMaterialRequest> materials = [];

    public void AddMaterial(int id, double modulusOfElasticity, double modulusOfRigidity) =>
        this.AddMaterials(
            new PutMaterialRequest()
            {
                Id = id,
                ModulusOfElasticity = modulusOfElasticity,
                ModulusOfRigidity = modulusOfRigidity,
                PressureUnit = this.UnitSettings.PressureUnit,
            }
        );

    public void AddMaterials(params Span<PutMaterialRequest> materials) =>
        this.materials.AddRange(materials);

    public override IEnumerable<PutMaterialRequest> MaterialRequests() =>
        this.materials.AsReadOnly();

    private readonly List<LoadCase> loadCases = [];

    public void AddLoadCase(int id, string caseName) =>
        this.AddLoadCases(new LoadCase() { Id = id, Name = caseName });

    public void AddLoadCases(params Span<LoadCase> loadCases) => this.loadCases.AddRange(loadCases);

    public override IEnumerable<LoadCase> LoadCaseRequests() => this.loadCases.AsReadOnly();

    private readonly List<LoadCombination> loadCombinations = [];

    public void AddLoadCombination(int id, params Span<(int, double)> loadCaseFactor) =>
        this.AddLoadCombinations(new LoadCombination(id, loadCaseFactor));

    public void AddLoadCombinations(params Span<LoadCombination> loadCombinations) =>
        this.loadCombinations.AddRange(loadCombinations);

    public override IEnumerable<LoadCombination> LoadCombinationRequests() =>
        this.loadCombinations.AsReadOnly();

    private readonly List<PutPointLoadRequest> pointLoads = [];

    public void AddPointLoad(int id, int nodeId, int loadCaseId, double force, Vector3 direction) =>
        this.AddPointLoads(
            new PutPointLoadRequest()
            {
                Id = id,
                NodeId = nodeId,
                LoadCaseId = loadCaseId,
                Force = new(force, this.UnitSettings.ForceUnit),
                Direction = direction,
            }
        );

    public void AddPointLoads(params Span<PutPointLoadRequest> pointLoads) =>
        this.pointLoads.AddRange(pointLoads);

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests() =>
        this.pointLoads.AsReadOnly();

    private readonly List<PutMomentLoadRequest> momentLoads = [];

    public void AddMomentLoad(
        int id,
        int nodeId,
        int loadCaseId,
        double moment,
        Vector3 axisDirection
    ) =>
        this.AddMomentLoads(
            new PutMomentLoadRequest()
            {
                Id = id,
                NodeId = nodeId,
                LoadCaseId = loadCaseId,
                Torque = new(moment, this.UnitSettings.TorqueUnit),
                AxisDirection = axisDirection,
            }
        );

    public void AddMomentLoads(params Span<PutMomentLoadRequest> momentLoads) =>
        this.momentLoads.AddRange(momentLoads);

    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests() =>
        this.momentLoads.AsReadOnly();

    private readonly List<PutSectionProfileRequest> sectionProfiles = [];

    public void AddSectionProfile(
        int id,
        string name,
        double area,
        double strongAxisMomentOfInertia,
        double weakAxisMomentOfInertia,
        double polarMomentOfInertia,
        double strongAxisPlasticSectionModulus,
        double weakAxisPlasticSectionModulus,
        double strongAxisShearArea,
        double weakAxisShearArea
    ) =>
        this.AddSectionProfiles(
            new PutSectionProfileRequest()
            {
                Id = id,
                Name = name,
                Area = area,
                StrongAxisMomentOfInertia = strongAxisMomentOfInertia,
                WeakAxisMomentOfInertia = weakAxisMomentOfInertia,
                PolarMomentOfInertia = polarMomentOfInertia,
                StrongAxisPlasticSectionModulus = strongAxisPlasticSectionModulus,
                WeakAxisPlasticSectionModulus = weakAxisPlasticSectionModulus,
                StrongAxisShearArea = strongAxisShearArea,
                WeakAxisShearArea = weakAxisShearArea,
                LengthUnit = this.UnitSettings.LengthUnit,
            }
        );

    public void AddSectionProfiles(params Span<PutSectionProfileRequest> sectionProfiles) =>
        this.sectionProfiles.AddRange(sectionProfiles);

    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests() =>
        this.sectionProfiles.AsReadOnly();

    private readonly List<SectionProfileFromLibrary> sectionProfilesFromLibrary = [];

    public void AddSectionProfileFromLibrary(int id, string name, StructuralCode library) =>
        this.AddSectionProfilesFromLibrary(
            new SectionProfileFromLibrary()
            {
                Id = id,
                Name = name,
                Library = library,
            }
        );

    public void AddSectionProfilesFromLibrary(
        params Span<SectionProfileFromLibrary> sectionProfilesFromLibrary
    ) => this.sectionProfilesFromLibrary.AddRange(sectionProfilesFromLibrary);

    public override IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibraryRequests() =>
        this.sectionProfilesFromLibrary.AsReadOnly();

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
using static BeamOs.StructuralAnalysis.Contracts.Common.AngleUnit;
using static BeamOs.StructuralAnalysis.Contracts.Common.LengthUnit;
using static BeamOs.StructuralAnalysis.Contracts.Common.PressureUnit;
using static BeamOs.StructuralAnalysis.Contracts.Common.AreaUnit;
using static BeamOs.StructuralAnalysis.Contracts.Common.AreaMomentOfInertiaUnit;

namespace {namespac};"
        );
        sb.AppendLine();
        sb.AppendLine(
            $"public partial class {className} : {baseClass ?? nameof(BeamOsModelBuilder)}"
        );
        sb.AppendLine("{");
        sb.AppendLine($"    public override string Name => nameof({className});");
        sb.AppendLine($"    public override string Description => \"{this.Description}\";");
        sb.AppendLine("    public override ModelSettings Settings => new ModelSettings");
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
        sb.AppendLine("        analysisSettings: new AnalysisSettings");
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
            sb.AppendLine($"            ModulusOfElasticity = {material.ModulusOfElasticity},");
            sb.AppendLine($"            ModulusOfRigidity = {material.ModulusOfRigidity},");
            sb.AppendLine($"            PressureUnit = {material.PressureUnit},");
            sb.AppendLine($"        }};");
        }
        sb.AppendLine("    }");
        sb.AppendLine();

        if (this.SectionProfileRequests().Any())
        {
            sb.AppendLine(
                "    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests()"
            );
            sb.AppendLine("    {");
            foreach (var sectionProfile in this.SectionProfileRequests())
            {
                sb.AppendLine($"        yield return new PutSectionProfileRequest");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            Id = {sectionProfile.Id},");
                sb.AppendLine($"            Name = {sectionProfile.Name},");
                sb.AppendLine($"            Area = {sectionProfile.Area},");
                sb.AppendLine(
                    $"            StrongAxisMomentOfInertia = {sectionProfile.StrongAxisMomentOfInertia},"
                );
                sb.AppendLine(
                    $"            WeakAxisMomentOfInertia = {sectionProfile.WeakAxisMomentOfInertia},"
                );
                sb.AppendLine(
                    $"            PolarMomentOfInertia = {sectionProfile.PolarMomentOfInertia},"
                );
                sb.AppendLine(
                    $"            StrongAxisShearArea = {sectionProfile.StrongAxisShearArea},"
                );
                sb.AppendLine(
                    $"            WeakAxisShearArea = {sectionProfile.WeakAxisShearArea},"
                );
                sb.AppendLine($"            LengthUnit = {sectionProfile.LengthUnit},");
                sb.AppendLine($"        }};");
            }

            sb.AppendLine("    }");
            sb.AppendLine();
        }

        if (this.SectionProfilesFromLibraryRequests().Any())
        {
            sb.AppendLine(
                "    public override IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibraryRequests()"
            );
            sb.AppendLine("    {");
            foreach (var sectionProfile in this.SectionProfilesFromLibraryRequests())
            {
                {
                    sb.AppendLine($"        yield return new SectionProfileFromLibrary");
                    sb.AppendLine($"        {{");
                    sb.AppendLine($"            Id = {sectionProfile.Id},");
                    sb.AppendLine($"            Name = \"{sectionProfile.Name}\",");
                    sb.AppendLine(
                        $"            Library = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile.StructuralCode.{sectionProfile.Library},"
                    );
                    sb.AppendLine($"        }};");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine();
        }
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
                sb.Append($"LoadCaseId = {pointLoad.LoadCaseId},");
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
                sb.AppendLine($"            LoadCaseId = {momentLoad.LoadCaseId},");
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
                $"model.def_support('N{node.Id}', {(!node.Restraint.CanTranslateAlongX).ToString()}, {(!node.Restraint.CanTranslateAlongY).ToString()}, {(!node.Restraint.CanTranslateAlongZ).ToString()}, {(!node.Restraint.CanRotateAboutX).ToString()}, {(!node.Restraint.CanRotateAboutY).ToString()}, {(!node.Restraint.CanRotateAboutZ).ToString()})"
            );
        }

        sb.AppendLine();
        foreach (var material in this.MaterialRequests())
        {
            sb.AppendLine(
                @$"
nu = 0.3  # Poisson's ratio
rho = 0.490/12**3  # Density (kci)
model.add_material('M{material.Id}', {new PressureContract(material.ModulusOfElasticity, material.PressureUnit).As(this.UnitSettings.PressureUnit)}, {new PressureContract(material.ModulusOfRigidity, material.PressureUnit).As(this.UnitSettings.PressureUnit)}, nu, rho)"
            );
        }
        sb.AppendLine();
        foreach (
            var sectionProfile in this.SectionProfileRequests().OfType<PutSectionProfileRequest>()
        )
        {
            sb.AppendLine(
                $"model.add_section('S{sectionProfile.Id}', {new AreaContract(sectionProfile.Area, sectionProfile.AreaUnit).As(this.Settings.UnitSettings.AreaUnit)}, {new AreaMomentOfInertiaContract(sectionProfile.WeakAxisMomentOfInertia, sectionProfile.AreaMomentOfInertiaUnit).As(this.UnitSettings.AreaMomentOfInertiaUnit)}, {new AreaMomentOfInertiaContract(sectionProfile.StrongAxisMomentOfInertia, sectionProfile.AreaMomentOfInertiaUnit).As(this.UnitSettings.AreaMomentOfInertiaUnit)}, {new AreaMomentOfInertiaContract(sectionProfile.PolarMomentOfInertia, sectionProfile.AreaMomentOfInertiaUnit).As(this.UnitSettings.AreaMomentOfInertiaUnit)})"
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
