using VizoMenuAPIv3.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public DateTime EnteredUTC { get; set; }
    public Guid EnteredById { get; set; } // Initially could reference itself for SuperAdmin
    public User? EnteredBy { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime? DisabledUTC { get; set; }
    public Guid? DisabledById { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
