using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend3.Storage
{
    public class Role : IdentityRole<Guid>
    {
        public RoleType Type { get; set; }
        public ICollection<UserRole> Users { get; set; }
    }

    public enum RoleType
    {
        [Display(Name = ApplicationRoleNames.Administrator)]
        Administrator,
        [Display(Name = ApplicationRoleNames.User)]
        User,
        [Display(Name = ApplicationRoleNames.Organizer)]
        Organizer
    }
    public class ApplicationRoleNames
    {
        public const string Administrator = "Администратор";
        public const string User = "Пользователь";
        public const string Organizer = "Организатор";
    }

    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

}
