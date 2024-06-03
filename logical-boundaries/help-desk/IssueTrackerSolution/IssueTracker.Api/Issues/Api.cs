using IssueTracker.Api.Catalog;
using IssueTracker.Api.Shared;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
            .Select(c => new IssueSoftwareEmbeddedResponse(c.Id, c.Title, c.Description))
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
            User = entity.User,
            Software = entity.Software
        };
        return Ok(response);
    }
}

public record UserCreateIssueRequestModel(string Description);

public record UserIssueResponse
{
    public Guid Id { get; set; } // the created issue ID
    public string User { get; set; } = string.Empty; // The user id, or the route to the user...
    public IssueSoftwareEmbeddedResponse? Software { get; set; } // the id of the software, or the route
    public IssueStatusType Status { get; set; } = IssueStatusType.Submitted;
}

public class UserIssue
{
    public Guid Id { get; set; } // the created issue ID
    public string User { get; set; } = string.Empty; // The user id, or the route to the user...
    public IssueSoftwareEmbeddedResponse? Software { get; set; } // the id of the software, or the route
    public IssueStatusType Status { get; set; } = IssueStatusType.Submitted;
    public DateTimeOffset Created { get; set; }

}
public record IssueSoftwareEmbeddedResponse(Guid Id, string Title, string Description);

public enum IssueStatusType { Submitted }
