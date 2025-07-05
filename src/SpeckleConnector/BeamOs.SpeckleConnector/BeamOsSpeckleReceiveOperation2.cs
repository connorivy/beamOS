using System.Runtime.CompilerServices;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.Extensions.DependencyInjection;
using Speckle.Objects.Data;
using Speckle.Sdk.Api;
using Speckle.Sdk.Logging;
using Speckle.Sdk.Models;
using Speckle.Sdk.Models.Collections;
using Speckle.Sdk.Models.Extensions;

namespace BeamOs.SpeckleConnector;

public class BeamOsSpeckleReceiveOperation2(
    IServiceProvider serviceProvider,
    SpeckleReceiveOperationContext context,
    ICommandHandler<
        ModelResourceRequest<ModelProposalData>,
        ModelProposalResponse
    > modelProposalCommandHandler,
    IOperations speckleOperations
) : ICommandHandler<ModelResourceRequest<SpeckleReceiveParameters>, ModelProposalResponse>
{
    // static BeamOsSpeckleReceiveOperation2()
    // {
    //     var speckleLoggerField =
    //         typeof(SpeckleLog).GetField(
    //             "s_logger",
    //             System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
    //         ) ?? throw new ArgumentException("Could not find private speckle logger field");

    //     // we need to assign a value to the speckle logger because the initialize method throws an exception.
    //     // it tries to use an MD5 hash which is not implemented in webAssembly.
    //     speckleLoggerField.SetValue(null, Serilog.Core.Logger.None);
    // }
    public async Task<Result<ModelProposalResponse>> ExecuteAsync(
        ModelResourceRequest<SpeckleReceiveParameters> request,
        CancellationToken ct = default
    )
    {
        // var account = new Account();
        // account.token = request.Body.ApiToken;
        // account.serverInfo = new ServerInfo { url = request.Body.ServerUrl };

        var commitObject = await speckleOperations.Receive2(
            new(request.Body.ServerUrl),
            request.Body.ProjectId,
            request.Body.ObjectId,
            request.Body.ApiToken,
            null,
            cancellationToken: ct
        );

        foreach (var item in commitObject.Flatten())
        {
            if (item is Collection collection)
            {
                var converter = serviceProvider.GetRequiredService<CollectionConverter>();
                _ = converter.ConvertAndReturnId(collection);
            }
        }

        foreach (var item in commitObject.Flatten()
        // this
        //     .graphTraversal.Traverse(commitObject)
        //     .Where(obj => obj.Current is not Collection)
        // .Freeze()
        )
        {
            if (this.GetConverter(item) is ToProposalConverter converter)
            {
                _ = converter.ConvertAndReturnId(item);
            }
        }

        return await modelProposalCommandHandler.ExecuteAsync(
            new() { ModelId = request.ModelId, Body = context.ProposalBuilder.Build() },
            ct
        );
    }

    private ToProposalConverter? GetConverter(Base @base)
    {
        if (@base.speckle_type != "Base")
        {
            ;
        }
        if (@base is RevitObject revitObject)
        {
            return revitObject switch
            {
                { category: "Structural Columns" } =>
                    serviceProvider.GetRequiredService<RevitColumnConverter>(),
                { category: "Structural Framing" } =>
                    serviceProvider.GetRequiredService<RevitBeamConverter>(),
                _ => null,
            };
        }
        return null;
    }
}
