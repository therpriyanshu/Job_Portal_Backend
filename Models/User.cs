using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ✅ Auto-increment ID
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string PasswordHash { get; set; }
}
