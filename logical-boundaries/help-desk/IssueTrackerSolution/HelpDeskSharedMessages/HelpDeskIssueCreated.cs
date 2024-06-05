namespace HelpDeskSharedMessages;

// What the IssueTracker API is going to send to the SupportApi
public class HelpDeskIssueCreated
{
    public Guid Id { get; set; }
    public Guid SoftwareId { get; set; }
    public Guid UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string SoftwareTitle { get; set; } = string.Empty;
    public string UserEmailAddress { get; set; } = string.Empty;


}
