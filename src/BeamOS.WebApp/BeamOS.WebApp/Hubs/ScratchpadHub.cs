using BeamOs.WebApp.Client.Components.Features.Scratchpad;
using Microsoft.AspNetCore.SignalR;

namespace BeamOS.WebApp.Hubs;

public class ScratchpadHub : Hub<IScratchpadHubClient>
{
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"\n\n\n\n\n\nConnection Id {this.Context.ConnectionId} \n\n\n\n\n\n");
        await base.OnConnectedAsync();
    }
}
