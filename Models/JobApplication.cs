public class JobApplication
{
    public Guid Id { get; set; }  // Primary Key
    public Guid JobId { get; set; }  // Foreign Key (Job)
    public string UserEmail { get; set; }  // Store user email instead of UserId
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

    public Job Job { get; set; }  // Navigation property
}
