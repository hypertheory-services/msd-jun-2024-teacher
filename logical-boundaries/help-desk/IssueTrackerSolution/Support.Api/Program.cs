using IssueTracker.Api.Issues;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


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

