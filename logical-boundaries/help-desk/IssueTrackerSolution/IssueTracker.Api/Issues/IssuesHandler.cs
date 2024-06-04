using Marten;

namespace IssueTracker.Api.Issues;

public class IssuesHandler
{

    public UserIssueCreated Handle(AddUserIssue command, IDocumentSession session)
    {
        // Turn a command into an event.
        var @event = new UserIssueCreated(command.Id, command.SoftwareId, command.UserId, command.Description, command.Impact);
        session.Events.Append(@event.Id, @event);
        return @event;
    }

}
public record UserIssueCreated(Guid Id, Guid SoftwareId, Guid UserId, string Description, IssueImpact Impact);