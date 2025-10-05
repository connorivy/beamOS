using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.ModelsPage;

[FeatureState]
public record ModelPageState
{
    [SetsRequiredMembers]
    public ModelPageState()
        : this([]) { }

    [SetsRequiredMembers]
    public ModelPageState(IReadOnlyCollection<ModelInfoResponse> modelResponses)
    {
        this.UserModelResponses = modelResponses;
    }

    public bool IsLoading { get; set; } = true;
    public required IReadOnlyCollection<ModelInfoResponse> UserModelResponses { get; init; }
    public static Guid TutorialGuid { get; }
    public static IReadOnlyCollection<ModelInfoResponse> SampleModelResponses { get; }

    static ModelPageState()
    {
        TutorialGuid = Guid.CreateVersion7();
        
        SampleModelResponses =
        [
            new(
                TutorialGuid,
                "Tutorial",
                "Learn the basics of BeamOS with this interactive tutorial",
                new ModelSettings(
                    unitSettings: new UnitSettingsContract
                    {
                        LengthUnit = LengthUnitContract.Foot,
                        ForceUnit = ForceUnitContract.KilopoundForce,
                        AngleUnit = AngleUnitContract.Radian,
                    },
                    analysisSettings: new AnalysisSettings
                    {
                        Element1DAnalysisType = Element1dAnalysisType.Timoshenko,
                    },
                    yAxisUp: true
                ),
                DateTimeOffset.Parse("2024-01-01T12:00:00Z"),
                "Sample"
            ),
            new(
                Guid.Parse("4ce66084-4ac1-40bc-99ae-3d0f334c66fa"),
                "Twisty Bowl Framing",
                "A crazy twisting bowl type structure. Made by Bjorn Steinhagen in grasshopper and then sent to beamOS using Speckle",
                new ModelSettings(
                    unitSettings: new UnitSettingsContract
                    {
                        LengthUnit = LengthUnitContract.Inch,
                        ForceUnit = ForceUnitContract.KilopoundForce,
                        AngleUnit = AngleUnitContract.Radian,
                    },
                    analysisSettings: new AnalysisSettings
                    {
                        Element1DAnalysisType = Element1dAnalysisType.Timoshenko,
                    },
                    yAxisUp: true
                ),
                DateTimeOffset.Parse("2023-11-01T12:00:00Z"),
                "Sample"
            ),
        ];
        // foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        // {
        //     // Only add sample models from SAP2000 because they look the coolest
        //     if (modelBuilder.SourceInfo?.SourceType != FixtureSourceType.SAP2000)
        //     {
        //         continue;
        //     }

        //     sampleModels.Add(
        //         new(
        //             modelBuilder.Id,
        //             modelBuilder.Name,
        //             modelBuilder.Description,
        //             modelBuilder.Settings,
        //             modelBuilder.LastModified,
        //             "Sample"
        //         )
        //     );
        // }
    }
}
