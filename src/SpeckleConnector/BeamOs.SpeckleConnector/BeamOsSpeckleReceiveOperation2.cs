using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Speckle.Core.Api.GraphQL.Models;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using Speckle.Core.Models.Extensions;
using Speckle.Core.Transports;

namespace BeamOs.SpeckleConnector;

public class BeamOsSpeckleReceiveOperation2(
    IServiceProvider serviceProvider,
    SpeckleReceiveOperationContext context,
    ICommandHandler<
        ModelResourceRequest<ModelProposalData>,
        ModelProposalResponse
    > modelProposalCommandHandler
) : ICommandHandler<ModelResourceRequest<SpeckleReceiveParameters>, ModelProposalResponse>
{
    static BeamOsSpeckleReceiveOperation2()
    {
        var speckleLoggerField =
            typeof(SpeckleLog).GetField(
                "s_logger",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            ) ?? throw new ArgumentException("Could not find private speckle logger field");

        // we need to assign a value to the speckle logger because the initialize method throws an exception.
        // it tries to use an MD5 hash which is not implemented in webAssembly.
        speckleLoggerField.SetValue(null, Serilog.Core.Logger.None);
    }

    public async Task<Result<ModelProposalResponse>> ExecuteAsync(
        ModelResourceRequest<SpeckleReceiveParameters> request,
        CancellationToken ct = default
    )
    {
        var account = new Account();
        account.token = request.Body.ApiToken;
        account.serverInfo = new ServerInfo { url = request.Body.ServerUrl };

        using ServerTransport transport = new(account, request.Body.ProjectId);
        Base commitObject = await Speckle
            .Core.Api.Operations.Receive(
                request.Body.ObjectId,
                transport,
                //localTransport: new DummyLocalTransport(),
                cancellationToken: ct
            )
            .ConfigureAwait(false);

        foreach (var item in commitObject.Flatten())
        {
            var proposalConverter = typeof(ITopLevelProposalConverter<>).MakeGenericType(
                item.GetType()
            );

            if (
                serviceProvider.GetService(proposalConverter)
                is ITopLevelProposalConverter converter
            )
            {
                _ = converter.ConvertAndReturnId(item);
            }
        }

        return await modelProposalCommandHandler.ExecuteAsync(
            new() { ModelId = request.ModelId, Body = context.ProposalBuilder.Build() },
            ct
        );
    }
}
