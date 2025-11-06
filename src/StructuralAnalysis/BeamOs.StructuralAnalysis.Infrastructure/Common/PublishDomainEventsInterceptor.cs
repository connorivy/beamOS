using BeamOs.Common.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

public class PublishDomainEventsInterceptor(IServiceProvider serviceProvider)
    : SaveChangesInterceptor
{
    private readonly List<IDomainEvent> domainEventsToHandleAfterSave = [];

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        await this.PublishDomainEvents(eventData.Context);
        return result;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var domainEvent in this.domainEventsToHandleAfterSave)
        {
            using var scope = serviceProvider.CreateScope();
            var notificationHandlerProvider =
                scope.ServiceProvider.GetRequiredService<DomainEventHandlerProvider>();
            var handler = notificationHandlerProvider.GetOptionalHandler(
                domainEvent.GetType(),
                scope.ServiceProvider
            );
            if (handler is not null)
            {
                await handler.HandleAsync(domainEvent);
            }
        }
        return result;
    }

    private async Task PublishDomainEvents(DbContext? dbContext)
    {
        Console.WriteLine("PublishDomainEventsInterceptor: Publishing domain events...");
        if (dbContext is null)
        {
            return;
        }

        var entitiesWithDomainEvents = dbContext
            .ChangeTracker.Entries<IHasDomainEvents>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToArray();

        var domainEvents = entitiesWithDomainEvents
            .SelectMany(entry => entry.DomainEvents)
            .ToArray();

        foreach (var entity in entitiesWithDomainEvents)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            Console.WriteLine(
                $"Publishing domain event: {domainEvent.GetType().Name} for entity: {domainEvent.GetType().Name}"
            );
            // using var scope = serviceProvider.CreateScope();
            var notificationHandlerProvider =
                // scope.ServiceProvider.GetRequiredService<DomainEventHandlerProvider>();
                serviceProvider.GetRequiredService<DomainEventHandlerProvider>();
            var handler = notificationHandlerProvider.GetOptionalHandler(
                domainEvent.GetType(),
                // scope.ServiceProvider
                serviceProvider
            );
            if (handler is not null)
            {
                if (handler.HandleAfterChangesSaved)
                {
                    this.domainEventsToHandleAfterSave.Add(domainEvent);
                    continue;
                }
                await handler.HandleAsync(domainEvent);
            }
        }
    }
}
