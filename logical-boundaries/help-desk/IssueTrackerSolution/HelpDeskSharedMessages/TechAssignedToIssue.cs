namespace HelpDeskSharedMessages;

// This is what the SupportApi is going to send back to us.

public class TechAssignedToIssue
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}