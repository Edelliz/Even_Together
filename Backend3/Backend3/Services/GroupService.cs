using Backend3.Models;
using Backend3.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend3.Services
{
    public interface IGroupService
    {
        Task Create(CreateGroupViewModel model, User user, Guid eventId);
        Task SendRequest(string email, Guid groupId);
        Task SendInvitation(string email, Guid userId, Guid eventId);
        Task AcceptInvitation(Guid groupId, string email);
        Task AcceptRequest(string email, Guid userId, Guid groupId);
        Task RefuseRequest(string email, Guid userId, Guid groupId);
        Task RefuseInvitation(Guid groupId, string email);
    }
    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;
        public GroupService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Create(CreateGroupViewModel model, User user, Guid eventId)
        {          
            var groupUser = await _context.Group.FirstOrDefaultAsync(x => x.EventId == eventId && x.Owner == user.Email);
            if(groupUser != null)
            {
                throw new Exception();
            }

            Group group = new Group
            {
                Title = model.Title,
                Description = model.Description,
                Size = model.Size,
                Owner = user.Email,
                EventId = eventId
            };

            
            var memder = new Member
            {
                GroupId = group.Id,
                UserId = user.Id
            };

            await _context.AddAsync(memder);
            await _context.Group.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task SendInvitation(string email, Guid userId, Guid eventId)
        {
            var group = await _context.Group.FirstOrDefaultAsync(x => x.EventId == eventId && x.Owner == email);
            if(group == null)
            {
                throw new Exception();
            }
            var invit = new Invitation
            {
                GroupId = group.Id,
                UserId = userId
            };
            await _context.AddAsync(invit);
            await _context.SaveChangesAsync();
        }

        public async Task SendRequest(string email, Guid groupId)
        {
            var request = new Request
            {
                GroupId = groupId,
                UserId = (await _userManager.FindByEmailAsync(email)).Id
            };
            await _context.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task AcceptInvitation(Guid groupId, string email)
        {
            var group = await _context.Group.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
            {
                throw new Exception();
            }
            var memder = new Member
            {
                GroupId = group.Id,
                UserId = (await _userManager.FindByEmailAsync(email)).Id
            };
            await _context.AddAsync(memder);
            var invite = _context.Invitations.FirstOrDefault(x => x.GroupId == groupId && x.UserId == memder.UserId);
            _context.Invitations.Remove(invite);

            await _context.SaveChangesAsync();
        }

        public async Task AcceptRequest(string email, Guid userId, Guid groupId)
        {
            var group = await _context.Group.FirstOrDefaultAsync(x => x.Id == groupId && x.Owner == email);
            if (group == null)
            {
                throw new Exception();
            }
            var memder = new Member
            {
                GroupId = group.Id,
                UserId = userId
            };
            await _context.AddAsync(memder);
            var request = _context.Request.FirstOrDefault(x => x.GroupId == groupId && x.UserId == memder.UserId);
            _context.Request.Remove(request);

            await _context.SaveChangesAsync();
        }

         public async Task RefuseInvitation(Guid groupId, string email)
        {
            var group = await _context.Group.FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null)
            {
                throw new Exception();
            }
            var userId = (await _userManager.FindByEmailAsync(email)).Id;
            var invite = _context.Invitations.FirstOrDefault( x => x.GroupId == groupId && x.UserId == userId);
            _context.Invitations.Remove(invite);

            await _context.SaveChangesAsync();
        }

        public async Task RefuseRequest(string email, Guid userId, Guid groupId)
        {
            var group = await _context.Group.FirstOrDefaultAsync(x => x.Id == groupId && x.Owner == email);
            if (group == null)
            {
                throw new Exception();
            }
            var request = _context.Request.FirstOrDefault(x => x.GroupId == groupId && x.UserId == userId);
            _context.Request.Remove(request);

            await _context.SaveChangesAsync();
        }
    }
}
