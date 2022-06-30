﻿using Microsoft.AspNetCore.Identity;

namespace Backend3.Storage
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string? Avatar { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        public ICollection<Invitation> Invitations { get; set; }
        public ICollection<Member> Member { get; set; }
    }

    public class Invitation
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }

    public class UsersEvents
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }
}
