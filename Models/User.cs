namespace snap_backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } 
        public required Role Role { get; set; }
        public ICollection<Package>? AssignedPackages { get; set; }
    }

    public enum Role
    {
        Courier,
        Admin
    }
}
