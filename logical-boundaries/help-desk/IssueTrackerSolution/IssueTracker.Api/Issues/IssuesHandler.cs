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



}

public class UserIssuePriorityHandler
{
    public async Task HandleAsync(AddUserIssue @event, IDocumentSession session, IMessageBus bus)
    {
        var user = await session.Query<UserInformation>().Where(u => u.Id == @event.Id).SingleAsync();
        var isCeo = user.Sub == "sue@aol.com";
        if (@event.Impact == IssueImpact.ProductionStoppage || @event.Impact == IssueImpact.WorkStoppage || isCeo)
        {
            session.Events.Append(@event.Id, new UserIssueCategorizedAsHighPriority(@event.Id));
            await bus.PublishAsync(new SendTextToAgentForThisIssue(@event.Id));
        }
    }
}
public record UserIssueCreated(Guid Id, Guid SoftwareId, Guid UserId, string Description, IssueImpact Impact);

public record UserIssueCategorizedAsHighPriority(Guid Id);

public record SendTextToAgentForThisIssue(Guid Id);