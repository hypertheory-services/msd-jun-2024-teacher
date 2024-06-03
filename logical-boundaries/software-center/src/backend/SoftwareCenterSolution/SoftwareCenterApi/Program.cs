
using Marten;
using Oakton;
using Oakton.Resources;
using SoftwareCatalogService.Outgoing;
using Wolverine;
using Wolverine.Http;
using Wolverine.Kafka;
using Wolverine.Marten;


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    WebRootPath = Path.Combine("wwwroot", "browser")
});
builder.Host.ApplyOaktonExtensions();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var connectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("No Connection String");
var kafkaUrl = builder.Configuration.GetConnectionString("kafka") ?? throw new Exception("No Kafka Connection String");
builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);
    
}).UseLightweightSessions().IntegrateWithWolverine();

builder.Host.UseWolverine(opts =>
{
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.AutoApplyTransactions();

    opts.UseKafka(kafkaUrl);
    opts.PublishMessage<SoftwareCatalogItemCreated>().ToKafkaTopic("softwarecenter.catalog-item-created");
    opts.PublishMessage<SoftwareCatalogItemRetired>().ToKafkaTopic("softwarecenter.catalog-item-retired");
    opts.Services.AddResourceSetupOnStartup();

});
var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("In Development");
    app.MapReverseProxy();
    
}
app.MapWolverineEndpoints();

return await app.RunOaktonCommands(args);
