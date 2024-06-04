using HelpDeskSharedMessages;
using IssueTracker.Api.Issues;
using Wolverine;
using Wolverine.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var kafkaConnectionString = builder.Configuration.GetValue<string>("kafka") ?? throw new Exception("Need a kafka broker");

builder.Host.UseWolverine(options =>
{
    options.UseKafka(kafkaConnectionString).ConfigureConsumers(c =>
    {
        c.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest; // that I've not seen.
        c.GroupId = "help-desk-support";
    });

    options.PublishMessage<TechAssignedToIssue>().ToKafkaTopic("help-desk.support-tech-assigned");
    options.ListenToKafkaTopic("help-desk.issue-created").ProcessInline(); // Look for a handler for this
                                                                           // 

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// We get that event that a HelpDeskIssueCreated arrives, and we will turn it into a TechAssignedToIssue

// Http Endpoing

app.MapGet("/", () =>
{
    var response = new SupportInfo
    {
        Name = "Graham",
        Phone = "855-1212",
        Email = "g@aol.com"
    };
    return TypedResults.Ok(response);
});


app.Run();



public class HandleNewIssuesHandler
{
    public TechAssignedToIssue Handle(HelpDeskIssueCreated issue, ILogger logger)
    {
        logger.LogInformation("Got an issue of {description} from {user}", issue.Description, issue.UserEmailAddress);
        return new TechAssignedToIssue
        {
            Id = issue.Id,
            Email = "g@aol.com",
            Phone = "555-1212",
            Name = "Grahm"
        };
    }
}