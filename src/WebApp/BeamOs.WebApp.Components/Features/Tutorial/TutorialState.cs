using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.Tutorial;

[FeatureState]
public record TutorialState(Guid ModelId)
{
    public TutorialState()
        : this(Guid.CreateVersion7()) { }

    public CreateModelRequest TutorialModelRequest =>
        new()
        {
            Id = this.ModelId,
            Name = "Tutorial Model",
            Description = "This model was created as part of the BeamOS tutorial.",
            Settings = new ModelSettings(UnitSettingsContract.K_IN),
        };

    public ModelInfoResponse TutorialModel =>
        new(
            this.ModelId,
            "Tutorial",
            "Learn the basics of BeamOS with this interactive tutorial",
            new ModelSettings(
                UnitSettingsContract.K_IN,
                analysisSettings: new AnalysisSettings
                {
                    Element1DAnalysisType = Element1dAnalysisType.Timoshenko,
                },
                yAxisUp: true
            ),
            DateTimeOffset.Parse("2024-01-01T12:00:00Z"),
            "Sample"
        );
}
