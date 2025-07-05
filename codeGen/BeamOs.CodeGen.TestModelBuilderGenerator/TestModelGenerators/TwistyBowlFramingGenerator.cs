using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.SpeckleConnector;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;

namespace BeamOs.CodeGen.TestModelBuilderGenerator.TestModelGenerators;

internal class TwistyBowlFramingGenerator(string speckleToken)
{
    private static string GuidString => "4ce66084-4ac1-40bc-99ae-3d0f334c66fa";
    private static ModelSettings Settings { get; } = new(UnitSettingsContract.K_IN);
    private static string Name => "TwistyBowlFraming";
    private static string Description =>
        "A crazy twisting bowl type structure. Made by Bjorn Steinhagen in grasshopper and then sent to beamOS using Speckle";

    private static string OutputPath => DirectoryHelper.GetSAP2000ProblemsDir();

    public async Task Generate()
    {
        BeamOsDynamicModelBuilder modelBuilder = new(GuidString, Settings, Name, Description);

        modelBuilder.AddSectionProfilesFromLibrary(
            new SectionProfileFromLibrary()
            {
                Id = 1636,
                Name = "w16x36",
                Library = StructuralCode.AISC_360_16,
            }
        );

        modelBuilder.AddMaterials(
            new PutMaterialRequest()
            {
                ModulusOfElasticity = 29000,
                ModulusOfRigidity = 11_153.85,
                PressureUnit = PressureUnitContract.KilopoundForcePerSquareInch,
                Id = 992,
            }
        );

        // var recieveOperation = new BeamOsSpeckleReceiveOperation2()
        // {
        //     Element1dRequestModifier = static el =>
        //         el with
        //         {
        //             MaterialId = 992,
        //             SectionProfileId = 1636,
        //         },
        //     NodeRequestModifier = static el =>
        //         el with
        //         {
        //             LocationPoint = new(
        //                 el.LocationPoint.X,
        //                 el.LocationPoint.Z,
        //                 -el.LocationPoint.Y,
        //                 el.LocationPoint.LengthUnit
        //             ),
        //         },
        // };

        // await foreach (
        //     IBeamOsEntityRequest request in recieveOperation.Receive(
        //         speckleToken,
        //         "d14016537b",
        //         "04efd5dd3d8ea53eb7c86062e47c8e10",
        //         "https://app.speckle.systems/"
        //     )
        // )
        // {
        //     if (request is PutNodeRequest putNodeRequest)
        //     {
        //         modelBuilder.AddNodes(putNodeRequest);
        //         if (
        //             new Length(
        //                 putNodeRequest.LocationPoint.Y,
        //                 putNodeRequest.LocationPoint.LengthUnit.MapEnumToLengthUnit()
        //             ).Millimeters > 10000
        //         )
        //         {
        //             modelBuilder.AddPointLoads(
        //                 new PutPointLoadRequest()
        //                 {
        //                     Id = putNodeRequest.Id,
        //                     LoadCaseId = 1,
        //                     Direction = new()
        //                     {
        //                         X = 0,
        //                         Y = -1,
        //                         Z = 0,
        //                     },
        //                     Force = new(100, ForceUnitContract.Kilonewton),
        //                     NodeId = putNodeRequest.Id,
        //                 }
        //             );
        //         }
        //     }
        //     else if (request is PutElement1dRequest putElement1DRequest)
        //     {
        //         modelBuilder.AddElement1ds(putElement1DRequest);
        //     }
        //     else
        //     {
        //         throw new NotImplementedException(
        //             $"Type {request.GetType()} is not implemented in speckle receive endpoint"
        //         );
        //     }
        // }

        modelBuilder.GenerateStaticModelClass(OutputPath, "ModelFixture");
        //modelBuilder.WriteToPyniteFile(OutputPath);
    }
}
