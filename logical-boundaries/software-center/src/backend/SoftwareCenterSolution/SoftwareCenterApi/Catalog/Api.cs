using Marten;
using Wolverine;
using Wolverine.Http;

namespace SoftwareCenterApi.Catalog;

public static class Api
{
    [WolverineGet("/api/catalog")]
    public static Task<IReadOnlyList<Data.CatalogItem>> Get(IQuerySession session) =>
        session.Query<Data.CatalogItem>().ToListAsync();
    [WolverinePost("/api/catalog")]
    public static async Task<IResult> Create(Models.CreateCatalogItemRequest request, IDocumentSession session, IMessageBus bus)
    {
        var item = new Data.CatalogItem()
        {
            Title = request.Title,
            Description = request.Description
        };
        session.Store(item);

        await bus.PublishAsync(new Events.CatalogItemCreated(item.Id));

        return Results.Created($"/api/catalog/{item.Id}", item);
    }

    [WolverineDelete(("/api/catalog/{id}"))]
    public static async Task<IResult> Delete(Guid id, IDocumentSession session, IMessageBus bus)
    {
         session.Delete<Data.CatalogItem>(id);
         await session.SaveChangesAsync();
         await bus.PublishAsync(new Events.CatalogItemRetired(id));
         return Results.Accepted();
    }

    [WolverineGet("/api/catalog/{id}")]
    public static async Task<IResult> GetById(Guid id, IDocumentSession session)
    {
        var item = await session.Query<Data.CatalogItem>().SingleOrDefaultAsync(c => c.Id == id);
        return item is not null ? Results.Ok(item) : Results.NotFound();
    }

    [WolverineGet("/api/catalog/{id}/issues")]
    public static async Task<IResult> GetIssuesById(Guid id, IDocumentSession session)
    {
        var item = await session.Query<Data.CatalogItem>().AnyAsync(c => c.Id == id);
        if (!item)
        {
            return Results.NotFound();
        }
        else
        {
            return Results.Ok(new List<string>());
        }
    }
}