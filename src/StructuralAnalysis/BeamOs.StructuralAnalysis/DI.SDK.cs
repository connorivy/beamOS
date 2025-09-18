using System.Net.Http.Headers;
using System.Text;
using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
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
#if !CODEGEN
        services.AddScoped<IStructuralAnalysisApiClientV2, StructuralAnalysisApiClientV2>();
#endif

        return services;
    }

    public static IServiceCollection AddBeamOsRemote(
        this IServiceCollection services,
        string apiToken
    )
    {
        // services.AddHttpClient();
        // AuthMessageHandler authMessageHandler = new(apiToken);
        // services.AddSingleton(authMessageHandler);

        // services
        //     .AddHttpClient<IStructuralAnalysisApiClientV1, StructuralAnalysisApiClientV1>(client =>
        //         client.BaseAddress = new("https://beamos.net/")
        //     )
        //     .AddHttpMessageHandler<AuthMessageHandler>();

        // services
        //     .AddHttpClient<ISpeckleConnectorApi, SpeckleConnectorApi>(client =>
        //         client.BaseAddress = new("https://beamos.net/")
        //     )
        //     .AddHttpMessageHandler<AuthMessageHandler>();

        return services;
    }

    public static IServiceCollection AddInMemoryInfrastructure(this IServiceCollection services)
    {
// #if !CODEGEN
//         // services.AddScoped<InMemoryApiClient2>();
//         // services.AddInMemoryCommandHandlers();
//         // services.AddScoped<IStructuralAnalysisApiClientV1, InMemoryApiClient>();
//         // services.AddScoped<InMemoryModelRepositoryStorage>();
//         services.AddScoped<InMemoryUnitOfWork>();
//         services.AddScoped<IStructuralAnalysisUnitOfWork>(sp =>
//             sp.GetRequiredService<InMemoryUnitOfWork>()
//         );
//         services.AddScoped<IModelRepository, InMemoryModelRepository>();
//         services.AddScoped<INodeDefinitionRepository, InMemoryNodeDefinitionRepository>();
//         services.AddScoped<INodeRepository, InMemoryNodeRepository>();
//         services.AddScoped<IInternalNodeRepository, InMemoryInternalNodeRepository>();
//         services.AddScoped<IMaterialRepository, InMemoryMaterialRepository>();
//         services.AddScoped<ISectionProfileRepository, InMemorySectionProfileRepository>();
//         services.AddScoped<
//             ISectionProfileFromLibraryRepository,
//             InMemorySectionProfileFromLibraryRepository
//         >();
//         services.AddScoped<IElement1dRepository, InMemoryElement1dRepository>();
//         services.AddScoped<IPointLoadRepository, InMemoryPointLoadRepository>();
//         services.AddScoped<IMomentLoadRepository, InMemoryMomentLoadRepository>();
//         services.AddScoped<ILoadCaseRepository, InMemoryLoadCaseRepository>();
//         services.AddScoped<ILoadCombinationRepository, InMemoryLoadCombinationRepository>();
//         services.AddScoped<INodeResultRepository, InMemoryNodeResultRepository>();
//         services.AddScoped<IResultSetRepository, InMemoryResultSetRepository>();
//         services.AddScoped<IEnvelopeResultSetRepository, InMemoryEnvelopeResultSetRepository>();
//         services.AddScoped<IModelProposalRepository, InMemoryModelProposalRepository>();
//         services.AddScoped<IProposalIssueRepository, InMemoryProposalIssueRepository>();
//         services.AddScoped<
//             IQueryHandler<EmptyRequest, ICollection<ModelInfoResponse>>,
//             InMemoryGetModelsQueryHandler
//         >();

//         services.AddScoped<
//             ICommandHandler<ModelResourceRequest<DateTimeOffset>, ModelResponse>,
//             InMemoryRestoreModeCommandHandler
//         >();
// #endif

        return services;
    }
}

// internal class InMemoryGetModelsQueryHandler
// {
// }

internal class AuthMessageHandler(string apiToken) : DelegatingHandler
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
