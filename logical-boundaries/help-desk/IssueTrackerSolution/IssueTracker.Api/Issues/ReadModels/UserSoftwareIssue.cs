using Marten.Events;
using Marten.Events.Aggregation;

namespace IssueTracker.Api.Issues.ReadModels;

public class UserSoftwareIssue
{
    public Guid Id { get; set; }
    public int Version { get; set; } // And I promise I'll talk about this in a bit.
    public Guid UserId { get; set; }
    public Guid SoftwareId { get; set; }
    public DateTimeOffset Created { get; set; }
    public string Description { get; set; } = string.Empty;
    public IssueStatusType Status { get; set; }

    public IList<StatusHistoryItem> StatusHistory { get; set; } = [];

}

public record StatusHistoryItem(IssueStatusType StatusAssigned, DateTimeOffset When);

public class UserIssueProjection : SingleStreamProjection<UserSoftwareIssue>
{
    public UserSoftwareIssue Create(IEvent<UserIssueCreated> @event)
    {
        return new UserSoftwareIssue
        {
            Id = @event.Id,
            Created = @event.Timestamp,
            Description = @event.Data.Description,
            SoftwareId = @event.Data.SoftwareId,
            Status = IssueStatusType.Submitted,
            StatusHistory = [new StatusHistoryItem(IssueStatusType.Submitted, @event.Timestamp)],
            UserId = @event.Data.UserId,
            Version = 1
        };
    }

    public UserSoftwareIssue Apply(IEvent<UserIssueCategorizedAsHighPriority> @event, UserSoftwareIssue issue)
    {
        issue.Status = IssueStatusType.SubmittedAsHighPriority;
        issue.StatusHistory = [
            .. issue.StatusHistory,
            new StatusHistoryItem(IssueStatusType.SubmittedAsHighPriority, @event.Timestamp)];
        return issue;
    }
}