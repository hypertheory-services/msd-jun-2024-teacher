using IssueTracker.Api.Shared;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Users;

public class Api(IDocumentSession session) : ControllerBase
{
    [HttpGet("/users/{id:guid}", Name = "users#get-by-id")]
    public async Task<ActionResult> GetUserFromIdAsync(Guid id, CancellationToken token)
    {
        var user = await session.LoadAsync<UserInformation>(id, token);
        if (user is null)
        {
            return NotFound();
        }
        else
        {
            var response = new UserInformationResponse
            {
                Id = user.Id,

            };
            return Ok(response);
        }
    }
}


public record UserInformationResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}