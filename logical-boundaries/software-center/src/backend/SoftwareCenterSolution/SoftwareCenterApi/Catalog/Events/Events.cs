namespace SoftwareCenterApi.Catalog.Events;

public record CatalogItemCreated(Guid Id);

public record CatalogItemRetired(Guid Id);