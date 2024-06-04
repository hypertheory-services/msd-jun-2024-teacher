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

// This is what the SupportApi is going to send back to us.

public class TechAssignedToIssue
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}