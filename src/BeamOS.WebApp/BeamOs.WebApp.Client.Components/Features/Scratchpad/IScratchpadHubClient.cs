using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Model;

namespace BeamOs.WebApp.Client.Components.Features.Scratchpad;

public interface IScratchpadHubClient
{
    public const string HubEndpointPattern = "/ScratchpadHub";
    public Task LoadEntityInViewer(BeamOsEntityContractBase entity);

    //public Task LoadEntityInViewer(ModelResponse entity);
    public Task LoadEntityInViewer(int entity);
}
