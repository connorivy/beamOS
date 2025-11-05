using BeamOs.Common.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Infrastructure;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface IDomainEventHandler
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
    public Task HandleAsync(IDomainEvent notification, CancellationToken ct = default);
    public bool HandleAfterChangesSaved { get; }
}

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface IDomainEventHandler<TNotification> : IDomainEventHandler
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    where TNotification : IDomainEvent
{
    public Task HandleAsync(TNotification notification, CancellationToken ct = default);
}

public abstract class DomainEventHandlerBase<TNotification> : IDomainEventHandler<TNotification>
    where TNotification : IDomainEvent
{
    public abstract Task HandleAsync(TNotification notification, CancellationToken ct = default);
    public virtual bool HandleAfterChangesSaved => false;

    public async Task HandleAsync(IDomainEvent notification, CancellationToken ct = default)
    {
        if (notification is not TNotification typedNotification)
        {
            throw new ArgumentException(
                $"Invalid notification type. Expected {typeof(TNotification)}, but got {notification.GetType()}"
            );
        }

        await this.HandleAsync(typedNotification, ct);
    }
}

public class DomainEventHandlerProvider
{
    private static readonly Dictionary<Type, Type> Handlers = [];

    public void RegisterHandler<TEvent, TEventHandler>(IServiceCollection services)
        where TEvent : IDomainEvent
    {
        var eventType = typeof(TEvent);
        var handlerType = typeof(TEventHandler);

        Handlers.Add(eventType, handlerType);
        services.AddScoped(handlerType);
    }

    public IDomainEventHandler GetHandler(Type eventType, IServiceProvider serviceProvider)
    {
        var type = eventType;
        if (!Handlers.TryGetValue(type, out var handler))
        {
            throw new KeyNotFoundException($"No handler registered for notification type {type}");
        }

        return (IDomainEventHandler)serviceProvider.GetRequiredService(handler);
    }

    public IDomainEventHandler? GetOptionalHandler(Type eventType, IServiceProvider serviceProvider)
    {
        var type = eventType;
        if (!Handlers.TryGetValue(type, out var handler))
        {
            return null;
        }

        return (IDomainEventHandler)serviceProvider.GetRequiredService(handler);
    }
}
