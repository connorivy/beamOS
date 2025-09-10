using System.Net.Http.Headers;
using System.Text;
using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Sdk;

public static class DI
{
    public static IServiceCollection AddStructuralAnalysisSdkRequired(
        this IServiceCollection services
    )
    {
        services.AddScoped<BeamOsResultApiClient>();
        services.AddScoped<BeamOsApiClient>();
        return services;
    }

    public static IServiceCollection AddBeamOsRemote(
        this IServiceCollection services,
        string apiToken
    )
    {
        services.AddHttpClient();
        AuthMessageHandler authMessageHandler = new(apiToken);
        services.AddSingleton(authMessageHandler);

        services
            .AddHttpClient<IStructuralAnalysisApiClientV1, StructuralAnalysisApiClientV1>(client =>
                client.BaseAddress = new("https://beamos.net/")
            )
            .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddScoped<IStructuralAnalysisApiClientV2, StructuralAnalysisApiClientV2>();

        services
            .AddHttpClient<ISpeckleConnectorApi, SpeckleConnectorApi>(client =>
                client.BaseAddress = new("https://beamos.net/")
            )
            .AddHttpMessageHandler<AuthMessageHandler>();

        return services;
    }

    public static IServiceCollection AddInMemoryInfrastructure(this IServiceCollection services)
    {
#if !CODEGEN
        // services.AddInMemoryCommandHandlers();
        // services.AddKeyedScoped<IStructuralAnalysisApiClientV1, InMemoryApiClient>("InMemory");
        services.AddScoped<InMemoryModelRepositoryStorage>();
        services.AddKeyedScoped<IStructuralAnalysisUnitOfWork, InMemoryUnitOfWork>("InMemory");
        services.AddKeyedScoped<IModelRepository, InMemoryModelRepository>("InMemory");
        services.AddKeyedScoped<INodeDefinitionRepository, InMemoryNodeDefinitionRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<INodeRepository, InMemoryNodeRepository>("InMemory");
        services.AddKeyedScoped<IInternalNodeRepository, InMemoryInternalNodeRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<IMaterialRepository, InMemoryMaterialRepository>("InMemory");
        services.AddKeyedScoped<ISectionProfileRepository, InMemorySectionProfileRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<
            ISectionProfileFromLibraryRepository,
            InMemorySectionProfileFromLibraryRepository
        >("InMemory");
        services.AddKeyedScoped<IElement1dRepository, InMemoryElement1dRepository>("InMemory");
        services.AddKeyedScoped<IPointLoadRepository, InMemoryPointLoadRepository>("InMemory");
        services.AddKeyedScoped<IMomentLoadRepository, InMemoryMomentLoadRepository>("InMemory");
        services.AddKeyedScoped<ILoadCaseRepository, InMemoryLoadCaseRepository>("InMemory");
        services.AddKeyedScoped<ILoadCombinationRepository, InMemoryLoadCombinationRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<INodeResultRepository, InMemoryNodeResultRepository>("InMemory");
        services.AddKeyedScoped<IResultSetRepository, InMemoryResultSetRepository>("InMemory");
        services.AddKeyedScoped<IEnvelopeResultSetRepository, InMemoryEnvelopeResultSetRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<IModelProposalRepository, InMemoryModelProposalRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<IProposalIssueRepository, InMemoryProposalIssueRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<
            IQueryHandler<EmptyRequest, ICollection<ModelInfoResponse>>,
            InMemoryGetModelsQueryHandler
        >("InMemory");

        services.AddKeyedScoped<
            ICommandHandler<ModelResourceRequest<DateTimeOffset>, ModelResponse>,
            InMemoryRestoreModeCommandHandler
        >("InMemory");
#endif

        return services;
    }
}

// internal class InMemoryGetModelsQueryHandler
// {
// }

public class AuthMessageHandler(string apiToken) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(apiToken))
        );
        return await base.SendAsync(request, cancellationToken);
    }
}
