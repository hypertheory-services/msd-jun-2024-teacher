using Google.Protobuf.WellKnownTypes;
using Marten;
using SoftwareCatalogService.Outgoing;
using SoftwareCenterApi.Catalog.Data;
using SoftwareCenterApi.Catalog.Events;
using Wolverine;

namespace SoftwareCenterApi.Catalog.Handlers;
public class CatalogHandler
{
    public async Task HandleAsync(CatalogItemCreated message, IDocumentSession session, IMessageBus bus)
    {
        var doc = await session.LoadAsync<CatalogItem>(message.Id);

        var @event = new SoftwareCatalogItemCreated
        {
            Id = doc.Id.ToString(),
            Name = doc.Title,
            Description = doc.Description,
            CreatedAt = Timestamp.FromDateTimeOffset(DateTimeOffset.Now)
        };
       
        await bus.PublishAsync(@event);
    }

    public async Task HandleAsync(CatalogItemRetired message, IMessageBus bus)
    {
        var @event = new SoftwareCatalogItemRetired()
        {
            Id = message.Id.ToString(),
            RetiredAt = Timestamp.FromDateTimeOffset(DateTimeOffset.Now)
        };

        await bus.PublishAsync(@event);
    }
}


