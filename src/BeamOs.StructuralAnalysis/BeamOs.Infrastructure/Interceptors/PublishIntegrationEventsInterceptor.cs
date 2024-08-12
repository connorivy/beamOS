using BeamOs.Common.Domain.Interfaces;
using BeamOs.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BeamOs.Infrastructure.Interceptors;

public class PublishIntegrationEventsInterceptor(IEventBus eventBus) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        await this.PublishIntegrationEvents(eventData.Context);
        return result;
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result) =>
        base.SavedChanges(eventData, result);

    private async Task PublishIntegrationEvents(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        var entitiesWithIntegrationEvents = dbContext
            .ChangeTracker
            .Entries<IHasIntegrationEvents>()
            .Where(entry => entry.Entity.IntegrationEvents.Any())
            .Select(entry => entry.Entity)
            .ToArray();

        var integrationEvents = entitiesWithIntegrationEvents
            .SelectMany(entry => entry.IntegrationEvents)
            .ToArray();

        foreach (var entity in entitiesWithIntegrationEvents)
        {
            entity.ClearIntegrationEvents();
        }

        foreach (var integrationEvent in integrationEvents)
        {
            await eventBus.PublishAsync(integrationEvent);
        }
    }
}
