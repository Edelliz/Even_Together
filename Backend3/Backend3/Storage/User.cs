using Microsoft.AspNetCore.Identity;

namespace Backend3.Storage
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        //public List<Event> Events { get; set; }
        public ICollection<UserRole> Roles { get; set; }
    }

    public class UsersEvents
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }
}
