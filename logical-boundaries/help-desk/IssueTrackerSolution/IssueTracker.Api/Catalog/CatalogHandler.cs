using Marten;
using SoftwareCatalogService.Outgoing;

namespace IssueTracker.Api.Catalog;

public class CatalogHandler
{
    public async Task HandleAsync(SoftwareCatalogItemCreated @event, IDocumentSession session)
    {
        var entity = new CatalogItem
        {
            Id = Guid.Parse(@event.Id),
            Title = @event.Name,
            Description = @event.Description,
            AddedBy = "software-center",
            CreatedAt = DateTimeOffset.UtcNow,

        };

        session.Store(entity);
        await session.SaveChangesAsync();
    }

    public async Task HandleAsync(SoftwareCatalogItemRetired @event, IDocumentSession session)
    {
        try
        {
            // crappy code to make a point... more performant ways to do this.
            var id = Guid.Parse(@event.Id);
            var savedItem = await session.Query<CatalogItem>().Where(c => c.Id == id).SingleAsync();

            savedItem.RemovedAt = DateTimeOffset.UtcNow;

            session.Store(savedItem);


            await session.SaveChangesAsync();
        }
        catch (Exception)
        {

            throw new RaceConditionException();
        }

    }
}

public class RaceConditionException : ApplicationException;
