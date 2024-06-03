using IssueTracker.Api.Catalog;
using IssueTracker.Api.Shared;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace IssueTracker.Api.Issues;
[ApiExplorerSettings(GroupName = "Issues")]
public class Api(UserIdentityService userIdentityService, IDocumentSession session) : ControllerBase
{


    // POST /catalog/{id}/issues
    [HttpPost("/catalog/{catalogItemId:guid}/issues")]
    [SwaggerOperation(Tags = ["Issues", "Software Catalog"])]
    [Authorize]
    public async Task<ActionResult<UserIssueResponse>> AddAnIssueAsync(
        Guid catalogItemId, [FromBody] UserCreateIssueRequestModel request, CancellationToken token)
    {

        var software = await session.Query<CatalogItem>()
            .Where(c => c.Id == catalogItemId)
            .Select(c => new IssueSoftwareEmbeddedResponse { Id = c.Id, Title = c.Title, Description = c.Description })
            .SingleOrDefaultAsync(token);
        if (software is null)
        {
            return NotFound("No Software With That Id In The Catalog.");
        }
        var userInfo = await userIdentityService.GetUserInformationAsync();
        var userUrl = Url.RouteUrl("users#get-by-id", new { id = userInfo.Id }) ?? throw new ChaosException("Need a User Url");

        var entity = new UserIssue
        {
            Id = Guid.NewGuid(),
            Status = IssueStatusType.Submitted,
            User = userUrl,
            Created = DateTimeOffset.Now,
            Software = software,

        };
        session.Store<UserIssue>(entity);
        await session.SaveChangesAsync(token);
        var response = new UserIssueResponse
        {
            Id = entity.Id,
            Status = entity.Status,

            Description = request.Description,
            SoftwareEmbedded = new EmbbeddedResource<IssueSoftwareEmbeddedResponse>
            {
                Links = [new Link { Rel = "self", Href = $"/catalog/{catalogItemId}" }],
                Data = software
            },
            Links = [

                new Link {
                    Rel= "support",
                    Href = $"/catalog/{catalogItemId}/issues/{entity.Id}/support"
                },
                new Link {
                    Rel = "user",
                    Href = entity.User
                }
                ]
        };
        return Ok(response);
    }

    [HttpGet("/catalog/{catalogItemId:guid}/issues/{issueId:guid}/support")]
    public async Task<ActionResult> GetSupportInfoAsync()
    {
        var response = new SupportInfo
        {
            Name = "Bob Smith",
            Email = "bob@company.com",
            Phone = "555-1212"
        };
        return Ok(response);
    }
}

public record UserCreateIssueRequestModel(string Description);

public record UserIssueResponse
{
    public Guid Id { get; set; } // the created issue ID
    public string Description { get; set; } = string.Empty;

    public IssueStatusType Status { get; set; } = IssueStatusType.Submitted;

    [JsonPropertyName("_links")]
    public IList<Link> Links { get; set; } = [];

    [JsonPropertyName("_embedded")]
    public EmbbeddedResource<IssueSoftwareEmbeddedResponse>? SoftwareEmbedded { get; set; }
}

public record Link
{
    public string Rel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;

}

public class EmbbeddedResource<T> where T : new()
{

    public T Data { get; set; } = new();
    [JsonPropertyName("_links")]
    public IList<Link> Links { get; set; } = [];

}

public class UserIssue
{
    public Guid Id { get; set; } // the created issue ID
    public string User { get; set; } = string.Empty; // The user id, or the route to the user...
    public IssueSoftwareEmbeddedResponse? Software { get; set; } // the id of the software, or the route
    public IssueStatusType Status { get; set; } = IssueStatusType.Submitted;
    public DateTimeOffset Created { get; set; }

}
public record IssueSoftwareEmbeddedResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public enum IssueStatusType { Submitted }


public record SupportInfo
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}