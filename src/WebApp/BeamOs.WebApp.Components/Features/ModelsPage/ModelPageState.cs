using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.Tests.Common;
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
    public static IReadOnlyCollection<ModelInfoResponse> SampleModelResponses { get; }

    static ModelPageState()
    {
        List<ModelInfoResponse> sampleModels = [];
        foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        {
            // Only add sample models from SAP2000 because they look the coolest
            if (modelBuilder.SourceInfo?.SourceType != FixtureSourceType.SAP2000)
            {
                continue;
            }

            sampleModels.Add(
                new(
                    modelBuilder.Id,
                    modelBuilder.Name,
                    modelBuilder.Description,
                    modelBuilder.Settings,
                    modelBuilder.LastModified,
                    "Sample"
                )
            );
        }
        SampleModelResponses = sampleModels;
    }
}
