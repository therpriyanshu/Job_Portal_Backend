public class Job
{
    public Guid Id { get; set; }  // Primary Key
    public string Title { get; set; }
    public string Description { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
}
