using HelpDeskSharedMessages;
using IssueTracker.Api.Catalog;
using IssueTracker.Api.Shared;
using Marten;
using Wolverine;

namespace IssueTracker.Api.Issues;

public class IssuesHandler // create a class that's name ends in Handler
{

    public UserIssueCreated Handle(AddUserIssue command, IDocumentSession session)
    {
        // Turn a command into an event.
        var @event = new UserIssueCreated(command.Id, command.SoftwareId, command.UserId, command.Description, command.Impact);
        session.Events.Append(@event.Id, @event);
        return @event;
    }

    public void Handle(UserIssueCreated command, ILogger logger)
    {
        logger.LogInformation("Got an issue... {d}", command.Description);
    }



}

public class UserIssuePriorityHandler
{
    public async Task HandleAsync(AddUserIssue @event, IDocumentSession session, IMessageBus bus)
    {
        var user = await session.Query<UserInformation>().Where(u => u.Id == @event.UserId).SingleAsync();
        var isCeo = user.Sub == "sue@aol.com";
        if (@event.Impact == IssueImpact.ProductionStoppage || @event.Impact == IssueImpact.WorkStoppage || isCeo)
        {
            session.Events.Append(@event.Id, new UserIssueCategorizedAsHighPriority(@event.Id));
            await bus.PublishAsync(new SendTextToAgentForThisIssue(@event.Id));
        }
    }
}

public class UserIssueTechAssignmentHandler
{
    public async Task<HelpDeskIssueCreated> HandleAsync(AddUserIssue @event, IDocumentSession session)
    {
        var title = await session.Query<CatalogItem>()
            .Where(c => c.Id == @event.SoftwareId)
            .Select(c => c.Title)
            .SingleAsync();

        var email = await session.Query<UserInformation>()
            .Where(c => c.Id == @event.UserId)
            .Select(c => c.Sub) // sub is subject, and it's the email address
            .SingleAsync();
        var response = new HelpDeskIssueCreated
        {
            Id = @event.Id,
            Description = @event.Description,
            SoftwareId = @event.SoftwareId,
            UserId = @event.UserId,
            SoftwareTitle = title,
            UserEmailAddress = email
        };
        return response;
    }
}
public record UserIssueCreated(Guid Id, Guid SoftwareId, Guid UserId, string Description, IssueImpact Impact);

public record UserIssueCategorizedAsHighPriority(Guid Id);

public record SendTextToAgentForThisIssue(Guid Id);