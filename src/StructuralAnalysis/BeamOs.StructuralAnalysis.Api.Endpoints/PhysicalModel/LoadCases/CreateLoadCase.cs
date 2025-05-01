using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Api;
using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class CreateLoadCase(CreateLoadCaseCommandHandler createLoadCaseCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreateLoadCaseCommand, LoadCaseData, LoadCase>
{
    public override async Task<Result<LoadCase>> ExecuteRequestAsync(
        CreateLoadCaseCommand req,
        CancellationToken ct = default
    ) => await createLoadCaseCommandHandler.ExecuteAsync(req, ct);
}

public record LoadCaseData
{
    [SetsRequiredMembers]
    public LoadCaseData(string name)
    {
        this.Name = name;
    }

    public LoadCaseData() { }

    public required string Name { get; set; }
}

public readonly struct CreateLoadCaseCommand : IModelResourceRequest<LoadCaseData>
{
    public Guid ModelId { get; init; }
    public LoadCaseData Body { get; init; }
    public string Name => this.Body.Name;
}

public record LoadCase : LoadCaseData, IHasIntId
{
    public required int Id { get; init; }
}
