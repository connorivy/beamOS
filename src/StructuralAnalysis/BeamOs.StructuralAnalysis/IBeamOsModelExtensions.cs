using System.Text;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
#if Sqlite || InMemory
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Sdk.Extensions;
#endif

namespace BeamOs.StructuralAnalysis.Sdk;

// todo: the new extension method syntax is causing this false error
#pragma warning disable CA1822 // Mark members as static

public static class IBeamOsModelExtensions
{
    extension(IBeamOsModel model)
    {
        /// <summary>
        /// Create the current model, but only if a model doesn't already exist in the current model repository
        /// (could be a local or online repository)
        /// </summary>
        /// <returns>a bool that is true if the model was created or false if it already existed</returns>
        public Task<bool> CreateOnly(BeamOsApiClient apiClient)
        {
            return model.CreateOnly(apiClient.InternalClient);
        }

        /// <summary>
        /// Create the current model, but only if a model doesn't already exist in the current model repository
        /// (could be a local or online repository)
        /// </summary>
        /// <returns>a bool that is true if the model was created or false if it already existed</returns>
        public Task<bool> CreateOnly(BeamOsResultApiClient apiClient)
        {
            return model.CreateOnly(apiClient.InternalClient);
        }

        internal async Task<bool> CreateOnly(IStructuralAnalysisApiClientV2 apiClientV1)
        {
            BeamOsModelBuilder builder = new(model, apiClientV1);
            return await builder.CreateOnly();
        }

        /// <summary>
        /// Create the current model if it doesn't exist in the current model repository, or update the
        /// model if it does exist. Only the elements added to the this model builder instance will be updated.
        /// Other existing elements in the model will not be affected.
        /// </summary>
        /// <returns>a bool that is true if the model was created or false if it already existed</returns>
        public Task<bool> CreateOrUpdate(BeamOsApiClient apiClient)
        {
            return model.CreateOrUpdate(apiClient.InternalClient);
        }

        /// <summary>
        /// Create the current model if it doesn't exist in the current model repository, or update the
        /// model if it does exist. Only the elements added to the this model builder instance will be updated.
        /// Other existing elements in the model will not be affected.
        /// </summary>
        /// <returns>a bool that is true if the model was created or false if it already existed</returns>
        public Task<bool> CreateOrUpdate(BeamOsResultApiClient apiClient)
        {
            return model.CreateOrUpdate(apiClient.InternalClient);
        }

        internal async Task<bool> CreateOrUpdate(IStructuralAnalysisApiClientV2 apiClientV1)
        {
            BeamOsModelBuilder builder = new(model, apiClientV1);
            return await builder.CreateOrUpdate();
        }

#if Sqlite || InMemory
        public void GenerateStaticModelClass(string outputDir, string? baseClass = null)
        {
            var sb = new StringBuilder();

            var namespac =
                "BeamOs.Tests.Common"
                + outputDir.Split("BeamOs.Tests.Common")[1].Replace("\\", ".").Replace("/", ".");
            var className = model.Name.Replace(" ", "");

#pragma warning disable
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
#pragma warning restore CA1305 // Specify IFormatProvider
            sb.AppendLine();
            sb.AppendLine(
                $"internal partial class {className} : {baseClass ?? nameof(BeamOsStaticModelBase)}"
            );
            sb.AppendLine("{");
            sb.AppendLine($"    internal override string Name => nameof({className});");
            sb.AppendLine($"    public override string Description => \"{model.Description}\";");
            sb.AppendLine("    public override ModelSettings Settings => new ModelSettings");
            sb.AppendLine("    (");
            sb.AppendLine("        unitSettings: new UnitSettingsContract");
            sb.AppendLine("        {");
            sb.AppendLine(
                $"            LengthUnit = LengthUnitContract.{model.Settings.UnitSettings.LengthUnit},"
            );
            sb.AppendLine(
                $"            ForceUnit = ForceUnitContract.{model.Settings.UnitSettings.ForceUnit},"
            );
            sb.AppendLine(
                $"            AngleUnit = AngleUnitContract.{model.Settings.UnitSettings.AngleUnit}"
            );
            sb.AppendLine("        },");
            sb.AppendLine("        analysisSettings: new AnalysisSettings");
            sb.AppendLine("        {");
            sb.AppendLine(
                $"            Element1DAnalysisType = Element1dAnalysisType.{model.Settings.AnalysisSettings.Element1DAnalysisType}"
            );
            sb.AppendLine("        },");
            sb.AppendLine($"        yAxisUp: {model.Settings.YAxisUp.ToString().ToLower()}");
            sb.AppendLine("    );");
            sb.AppendLine($"    public override Guid Id => Guid.Parse(\"{model.Id.ToString()}\");");
            sb.AppendLine();
            sb.AppendLine("    public override IEnumerable<PutNodeRequest> NodeRequests()");
            sb.AppendLine("    {");
            foreach (var node in model.NodeRequests())
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
            foreach (var material in model.MaterialRequests())
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

            if (model.SectionProfileRequests().Any())
            {
                sb.AppendLine(
                    "    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests()"
                );
                sb.AppendLine("    {");
                foreach (var sectionProfile in model.SectionProfileRequests())
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

            if (model.SectionProfilesFromLibraryRequests().Any())
            {
                sb.AppendLine(
                    "    public override IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibraryRequests()"
                );
                sb.AppendLine("    {");
                foreach (var sectionProfile in model.SectionProfilesFromLibraryRequests())
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
            foreach (var element in model.Element1dRequests())
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

            var pointLoads = model.PointLoadRequests().ToList();
            if (pointLoads.Count > 0)
            {
                sb.AppendLine(
                    "    public override IEnumerable<PutPointLoadRequest> PointLoadRequests()"
                );
                sb.AppendLine("    {");
                foreach (var pointLoad in pointLoads)
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

            var momentLoads = model.MomentLoadRequests().ToList();
            if (momentLoads.Count > 0)
            {
                sb.AppendLine(
                    "    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests()"
                );
                sb.AppendLine("    {");
                foreach (var momentLoad in momentLoads)
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

            var className = model.Name.Replace(" ", "");

            sb.AppendLine(
                @"
from Pynite import FEModel3D
import time

model = FEModel3D()"
            );

            foreach (var node in model.NodeRequests())
            {
                sb.AppendLine(
                    $"model.add_node('N{node.Id}', {node.LocationPoint.X.Convert(node.LocationPoint.LengthUnit, model.Settings.UnitSettings.LengthUnit)}, {node.LocationPoint.Y.Convert(node.LocationPoint.LengthUnit, model.Settings.UnitSettings.LengthUnit)}, {node.LocationPoint.Z.Convert(node.LocationPoint.LengthUnit, model.Settings.UnitSettings.LengthUnit)})"
                );
                sb.AppendLine(
                    $"model.def_support('N{node.Id}', {(!node.Restraint.CanTranslateAlongX).ToString()}, {(!node.Restraint.CanTranslateAlongY).ToString()}, {(!node.Restraint.CanTranslateAlongZ).ToString()}, {(!node.Restraint.CanRotateAboutX).ToString()}, {(!node.Restraint.CanRotateAboutY).ToString()}, {(!node.Restraint.CanRotateAboutZ).ToString()})"
                );
            }

            sb.AppendLine();
            foreach (var material in model.MaterialRequests())
            {
                sb.AppendLine(
                    @$"
nu = 0.3  # Poisson's ratio
rho = 0.490/12**3  # Density (kci)
model.add_material('M{material.Id}', {new PressureContract(material.ModulusOfElasticity, material.PressureUnit).As(model.UnitSettings.PressureUnit)}, {new PressureContract(material.ModulusOfRigidity, material.PressureUnit).As(model.UnitSettings.PressureUnit)}, nu, rho)"
                );
            }
            sb.AppendLine();
            foreach (
                var sectionProfile in model.SectionProfileRequests().OfType<PutSectionProfileRequest>()
            )
            {
                sb.AppendLine(
                    $"model.add_section('S{sectionProfile.Id}', {new AreaContract(sectionProfile.Area, sectionProfile.AreaUnit).As(model.Settings.UnitSettings.AreaUnit)}, {new AreaMomentOfInertiaContract(sectionProfile.WeakAxisMomentOfInertia, sectionProfile.AreaMomentOfInertiaUnit).As(model.UnitSettings.AreaMomentOfInertiaUnit)}, {new AreaMomentOfInertiaContract(sectionProfile.StrongAxisMomentOfInertia, sectionProfile.AreaMomentOfInertiaUnit).As(model.UnitSettings.AreaMomentOfInertiaUnit)}, {new AreaMomentOfInertiaContract(sectionProfile.PolarMomentOfInertia, sectionProfile.AreaMomentOfInertiaUnit).As(model.UnitSettings.AreaMomentOfInertiaUnit)})"
                );
            }
            sb.AppendLine();
            foreach (var element in model.Element1dRequests())
            {
                sb.AppendLine(
                    $"model.add_member('El{element.Id}', 'N{element.StartNodeId}', 'N{element.EndNodeId}', 'M{element.MaterialId}', 'S{element.SectionProfileId}')"
                );
            }
            sb.AppendLine();

            foreach (var pointLoad in model.PointLoadRequests())
            {
                var (pyniteDir, forceMult) = BeamOsDirectionToPyniteStringAndForceMultiplier(
                    pointLoad.Direction
                );
                sb.AppendLine(
                    $"model.add_node_load('N{pointLoad.NodeId}', 'F{pyniteDir}', {forceMult * pointLoad.Force.As(model.UnitSettings.ForceUnit)}, case='D')"
                );
            }
            sb.AppendLine();

            foreach (var momentLoad in model.MomentLoadRequests())
            {
                var (pyniteDir, forceMult) = BeamOsDirectionToPyniteStringAndForceMultiplier(
                    momentLoad.AxisDirection
                );
                sb.AppendLine(
                    $"model.add_node_load('N{momentLoad.NodeId}', 'M{pyniteDir}', {forceMult * momentLoad.Torque.As(model.UnitSettings.TorqueUnit)}, case='D')"
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
#endif
    }
    // public static Task Create(this IBeamOsModel model)
    // {

    //  }
}

#pragma warning restore CA1822 // Mark members as static
