using Backend3.Models;

namespace Backend3.Storage
{
    public class Group
    {
        public Guid Id { get; set; } = new Guid();
        public Guid EventId { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ICollection<Member> Users { get; set; } = new List<Member>();
        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
        public int Size { get; set; }
    }

    public class Member
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
    public class Request
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
