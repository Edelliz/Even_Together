using Backend3.Models;
using Backend3.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend3.Services
{
    public interface IAccountService
    {
        Task<UserViewModel> GetUser(Guid id, string email);
        Task Edit(EditUserViewModel model);
        Task<EditUserViewModel> GetUserView(Guid id);
    }
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<User> _userManager;
        private readonly IEventService _eventService;

        private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };
        public AccountService(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<User> userManager, IEventService eventService)
        {
            _context = context;
            _environment = environment;
            _eventService = eventService;
        }

        public async Task<UserViewModel> GetUser(Guid id, string email)
        {
            var user = await Get(id);

            bool isOwner = false;
            if(user.Email == email)
            {
                isOwner = true;
            }
            return new UserViewModel
            {
                Name = user.FullName,
                Email = user.Email,
                BirthDate = user.BirthDate,
                Avatar = user.Avatar,
                Id = id,
                IsOwner = isOwner,
                UsersEvents = await GetEvents(id),
                Invitations = await GetInvitations(id),
                Requests = await GetRequestsUser(id)
            };
        }

        public async Task<List<GroupViewModel>> GetRequestsUser(Guid id)
        {
            var user = _context.Users.Find(id);
            var groups = _context.Group.Where(x => x.Owner == user.Email);
            List<GroupViewModel> GroupViewModel = new List<GroupViewModel>();
            foreach (var group in groups)
            {
                var requests = _context.Request.Include(x => x.Group).Where(x => x.GroupId == group.Id);
                
                foreach (var request in requests)
                {
                    GroupViewModel.Add(new GroupViewModel
                    {
                        Id = request.GroupId,
                        Title = request.Group.Title,
                        Description = request.Group.Description,
                        Owner = request.Group.Owner,
                        Users = await _eventService.GetMembers(request.GroupId),
                        Requests = await _eventService.GetRequests(request.GroupId),
                        Size = request.Group.Size,
                    });
                }
            }
           
            return GroupViewModel;
        }
        public async Task<List<GroupViewModel>> GetInvitations(Guid id)
        {
            var invitations = _context.Invitations.Include(x => x.Group).Where(x => x.UserId == id);
            List<GroupViewModel> GroupViewModel = new List<GroupViewModel>();
            foreach (var invite in invitations)
            {
                GroupViewModel.Add(new GroupViewModel
                {
                    Id = invite.GroupId,
                    Title = invite.Group.Title,
                    Description = invite.Group.Description,
                    Owner = request.Group.Owner,
                    Users = await _eventService.GetMembers(invite.GroupId),
                    Requests = await _eventService.GetRequests(invite.GroupId),
                    Size = invite.Group.Size,
                });
            }
            return GroupViewModel;
        }
        private async Task<List<ShortEventViewModel>> GetEvents(Guid id)
        {
            var roleId = (await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == id)).RoleId;

            IQueryable<Event> events;
            if ((await _context.Roles.FirstOrDefaultAsync(x => x.Name == "Организатор")).Id == roleId)
            {
                var user = await Get(id);
                events = _context.Event.Where(x => x.Organizer == user.Email);
            }
            else
            {
                var usersEvent = _context.UsersEvents.Where(x => x.UserId == id);
                events = _context.Event.Where(x => usersEvent.Any(y => y.EventId == x.Id));
            }
           
            
            return await events.Select(x => new ShortEventViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Poster = x.Poster
            }).ToListAsync();
        }
        private async Task<User> Get(Guid? id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with Id={id} does not found!");
            }
            return user;
        }
        public async Task<EditUserViewModel> GetUserView(Guid id)
        {
            var user = await Get(id);
            return new EditUserViewModel
            {
                BirthDate = user.BirthDate,
                Name = user.FullName,
                Email = user.Email,
            };
        }

        public async Task Edit(EditUserViewModel model)
        {
            var user = await Get(model.Id);
            user.BirthDate = model.BirthDate;
            user.FullName = model.Name;
            user.Email = model.Email;

            var fileNameWithPath = await AddFile(model.Avatar);
            if (fileNameWithPath is not null)
            {
                if (File.Exists("wwwroot/" + user.Avatar))
                {
                    File.Delete("wwwroot/" + user.Avatar);
                }

                user.Avatar = fileNameWithPath;
            }
            await _context.SaveChangesAsync();
        }
        public async Task<string> AddFile(IFormFile file)
        {
            var isFileAttached = file != null;
            string fileNameWithPath = null;
            if (isFileAttached)
            {
                var extension = Path.GetExtension(file.FileName).Replace(".", "");
                if (!AllowedExtensions.Contains(extension))
                {
                    throw new ArgumentException("Attached file has not supported extension");
                }
                fileNameWithPath = $"files/{Guid.NewGuid()}-{file.FileName}";
                using (var fs = new FileStream(Path.Combine(_environment.WebRootPath, fileNameWithPath), FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
            }
            return fileNameWithPath;
        }
    }
}
