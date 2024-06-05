using FluentValidation;
using HelpDeskSharedMessages;
using IssueTracker.Api.Catalog;
using IssueTracker.Api.Issues.ReadModels;
using IssueTracker.Api.Shared;
using JasperFx.Core;
using Marten;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Oakton;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Kafka;
using Wolverine.Marten;



var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyOaktonExtensions();



builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
// sets up the auth stuff to read from our environment specific config.
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddScoped<IAuthorizationHandler, ShouldBeCreatorOfCatalogItemRequirementHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserIdentityService>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsSoftwareAdmin", policy =>
    {
        policy.RequireRole("SoftwareCenter");
        policy.AddRequirements(new ShouldBeCreatorToAlterCatalogItemRequirement());
    });
});

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddSwaggerGen(options =>
{


    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header with bearer token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",
                Name = "Bearer ",
                In = ParameterLocation.Header
            },[]
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
    options.EnableAnnotations();
}); // this will add the stuff to generate an OpenApi specification.
//builder.Services.AddSingleton<IValidator<CreateCatalogItemRequest>, CreateCatalogItemRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCatalogItemRequestValidator>();

var connectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("Can't start, need a connection string");

builder.Services.AddMarten(options =>
{
    options.UseSystemTextJsonForSerialization();
    options.Connection(connectionString);
    options.DatabaseSchemaName = "issues";
    options.Projections.Add<UserIssueProjection>(Marten.Events.Projections.ProjectionLifecycle.Inline);
}).UseLightweightSessions().IntegrateWithWolverine().AddAsyncDaemon(Marten.Events.Daemon.Resiliency.DaemonMode.Solo);

var kafkaConnectionString = builder.Configuration.GetValue<string>("kafka") ?? throw new Exception("Need a kafka broker");
builder.Host.UseWolverine(opts =>
{
    opts.OnException<RaceConditionException>().RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 5.Minutes());
    opts.UseKafka(kafkaConnectionString).ConfigureConsumers(c =>
    {
        c.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest; // that I've not seen.
        c.GroupId = "help-desk-issue-tracker-tacos";
    }); ; // more here later

    opts.Policies.AutoApplyTransactions();
    opts.ListenToKafkaTopic("softwarecenter.catalog-item-created").ProcessInline();
    opts.ListenToKafkaTopic("softwarecenter.catalog-item-retired").ProcessInline();
    opts.PublishMessage<HelpDeskIssueCreated>().ToKafkaTopic("help-desk.issue-created");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // This adds the ability to get the OpenAPI Spec through the API at https://localhost:1338/swagger/v1/swagger.json
    app.UseSwaggerUI(); // This adds "SwaggerUI" - a web application that reads that OpenAPI spec above and puts a pretty UI on it.
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization(); // come back to this.

app.MapControllers(); // create the call sheet. 

app.MapReverseProxy(); // any request coming in, look at the YARP config to see if they should go somewhere else


return await app.RunOaktonCommands(args);

public partial class Program { }