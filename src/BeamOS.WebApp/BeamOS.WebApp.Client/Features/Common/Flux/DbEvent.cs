using System.Diagnostics.CodeAnalysis;
using BeamOs.IntegrationEvents.Common;

namespace BeamOS.WebApp.Client.Features.Common.Flux;

public readonly struct DbEvent : IIntegrationEventWrapper
{
    [SetsRequiredMembers]
    public DbEvent(IIntegrationEvent integrationEvent)
    {
        this.IntegrationEvent = integrationEvent;
    }

    public required IIntegrationEvent IntegrationEvent { get; init; }
}

public readonly struct HistoryEvent : IIntegrationEventWrapper
{
    [SetsRequiredMembers]
    public HistoryEvent(IIntegrationEvent integrationEvent)
    {
        this.IntegrationEvent = integrationEvent;
    }

    public required IIntegrationEvent IntegrationEvent { get; init; }
}

public readonly struct EditorEvent : IIntegrationEventWrapper
{
    [SetsRequiredMembers]
    public EditorEvent(IIntegrationEvent integrationEvent)
    {
        this.IntegrationEvent = integrationEvent;
    }

    public required IIntegrationEvent IntegrationEvent { get; init; }
}

public interface IIntegrationEventWrapper
{
    public IIntegrationEvent IntegrationEvent { get; init; }
}
