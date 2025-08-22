using TUnit.Core;

namespace BeamOs.Tests.SourceGen;

public class SdkClientGeneratorTests
{
    [Test]
    public Task GenerateSdkClient()
    {
        string source = """
using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.SectionProfiles;

[BeamOsRoute(
    RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/{id:int}/from-library"
)]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutSectionProfileFromLibrary(
    PutSectionProfileFromLibraryCommandHandler putSectionProfileCommandHandler
)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutSectionProfileFromLibraryCommand,
        SectionProfileFromLibraryData,
        SectionProfileFromLibrary
    >
{
    public override async Task<Result<SectionProfileFromLibrary>> ExecuteRequestAsync(
        PutSectionProfileFromLibraryCommand req,
        CancellationToken ct = default
    ) => await putSectionProfileCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/from-library")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutSectionProfileFromLibrary(
    BatchPutSectionProfileFromLibraryCommandHandler putSectionProfileFromLibraryCommandHandler
)
    : BeamOsModelResourceBaseEndpoint<
        BatchPutSectionProfileFromLibraryCommand,
        SectionProfileFromLibrary[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutSectionProfileFromLibraryCommand req,
        CancellationToken ct = default
    ) => await putSectionProfileFromLibraryCommandHandler.ExecuteAsync(req, ct);
}

""";

        var commandHandlerSource = $$"""
using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public class PutSectionProfileFromLibraryCommandHandler(
    ISectionProfileRepository sectionProfileRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PutSectionProfileFromLibraryCommand, SectionProfileFromLibraryContract>
{
    public async Task<Result<SectionProfileFromLibraryContract>> ExecuteAsync(
        PutSectionProfileFromLibraryCommand command,
        CancellationToken ct = default
    )
    {
        SectionProfile? sectionProfile = SectionProfile.FromLibraryValue(
            command.ModelId,
            command.Library,
            command.Name
        );
        if (sectionProfile is null)
        {
            return BeamOsError.NotFound(
                description: $"Section profile with code {command.Library} not found."
            );
        }
        await sectionProfileRepository.Put(sectionProfile);
        await unitOfWork.SaveChangesAsync(ct);

        return new SectionProfileFromLibraryContract()
        {
            Id = sectionProfile.Id,
            Name = command.Name,
            Library = command.Library,
        };
    }
}

public readonly struct PutSectionProfileFromLibraryCommand
    : IModelResourceWithIntIdRequest<SectionProfileFromLibraryData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public SectionProfileFromLibraryData Body { get; init; }
    public StructuralCode Library => this.Body.Library;
    public string Name => this.Body.Name;

    public PutSectionProfileFromLibraryCommand() { }

    [SetsRequiredMembers]
    public PutSectionProfileFromLibraryCommand(
        ModelId modelId,
        SectionProfileFromLibraryContract putSectionProfileFromLibraryRequest
    )
    {
        this.Id = putSectionProfileFromLibraryRequest.Id;
        this.ModelId = modelId;
        this.Body = putSectionProfileFromLibraryRequest;
    }
}

public class BatchPutSectionProfileFromLibraryCommandHandler(
    ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        SectionProfileId,
        SectionProfileFromLibrary,
        BatchPutSectionProfileFromLibraryCommand,
        SectionProfileFromLibraryContract
    >(sectionProfileFromLibraryRepository, unitOfWork)
{
    protected override SectionProfileFromLibrary ToDomainObject(
        ModelId modelId,
        SectionProfileFromLibraryContract putRequest
    ) => new PutSectionProfileFromLibraryCommand(modelId, putRequest).ToDomainObject();
}

public readonly struct BatchPutSectionProfileFromLibraryCommand
    : IModelResourceRequest<SectionProfileFromLibraryContract[]>
{
    public Guid ModelId { get; init; }
    public SectionProfileFromLibraryContract[] Body { get; init; }
}

""";

        return SnapshotVerifier.Verify(result => result.Results.First().GeneratedSources.First().SourceText.ToString(), commandHandlerSource, source);
    }
}
