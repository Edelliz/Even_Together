using Backend3.Models;
using Backend3.Storage;
using Microsoft.EntityFrameworkCore;

namespace Backend3.Services
{
    public interface IEventService
    {
        Task CreateEvent(CreateEventViewModel model, string email);
        Task ChangeEvent(CreateEventViewModel model);
        Task<List<ShortEventViewModel>> GetAllEvent(DateTime? data);
        Task<CreateEventViewModel> GetEventView(Guid id);
        Task FindCompany(string email, Guid id);
        Task BuyTicket(string email, Guid id);
        Task Rate(Guid id, int grade);
        Task PostReview(Guid id, string text, string email);
        Task<List<ShortUserViewModel>> GetSearching(Guid eventId);
        Task<EventViewModel> GetEvent(Guid id, string userEmail);
        Task<List<ShortUserViewModel>> GetMembers(Guid id);
        Task<List<ShortUserViewModel>> GetRequests(Guid id);
    }
    public class EventService : IEventService
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };
        public EventService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        public async Task ChangeEvent(CreateEventViewModel model)
        {
            var ev = await Get(model.Id);
            ev.Date = model.Date;
            ev.Title = model.Title;
            ev.Description = model.Description;
            ev.Price = model.Price;
            ev.Place = model.Place;

            var fileNameWithPath = await AddFile(model.Poster);
            if (fileNameWithPath is not null)
            {
                if (File.Exists("wwwroot/" + ev.Poster))
                {
                    File.Delete("wwwroot/" + ev.Poster);
                }

                ev.Poster = fileNameWithPath;
            }
            await _context.SaveChangesAsync();
        }

        public async Task CreateEvent(CreateEventViewModel model, string email)
        {
            var fileNameWithPath = await AddFile(model.Poster);
            Event ev = new Event
            {
                Title = model.Title,
                Date = model.Date,
                Description = model.Description,
                Price = model.Price,
                Organizer = email,
                Poster = fileNameWithPath,
                Place = model.Place,
            };

            await _context.Event.AddAsync(ev);
            await _context.SaveChangesAsync();
        }


        public async Task<EventViewModel> GetEvent(Guid id, string userEmail)
        {
            var ev = await Get(id);

            bool isOWner = false;
            if (ev.Organizer == userEmail)
            {
                isOWner = true;
            }

            return new EventViewModel
            {
                Description = ev.Description,
                Title = ev.Title,
                Id = id,
                Date = ev.Date,
                Poster = ev.Poster,
                Price = ev.Price,
                Organizer = ev.Organizer,
                IsOwner = isOWner,
                Searching = await GetSearching(id),
                Grade = ev.Grade,
                Place = ev.Place,
                Reviews = await GetReviews(id),
                Groups = await GetGroup(id),
            };
        }
        public async Task<List<GroupViewModel>> GetGroup(Guid id)
        {
            var groups =  _context.Group.Where(g => g.EventId == id).ToList();
            List<GroupViewModel> groupViewModels = new List<GroupViewModel>();
            foreach(var group in groups)
            {
                groupViewModels.Add(new GroupViewModel
                {
                    Id = group.Id,
                    Title = group.Title,
                    Description = group.Description,
                    Owner = group.Owner,
                    Users = await GetMembers(group.Id),
                    Requests = await GetRequests(group.Id),
                    Size = group.Size,
                });
            }
            return groupViewModels;
        }
        public async Task<List<ShortUserViewModel>> GetRequests(Guid id)
        {
            var users = _context.Request.Include(x => x.User).Where(x => x.GroupId == id);
            List<ShortUserViewModel> requestViewModels = new List<ShortUserViewModel>();
            foreach (var user in users)
            {
                requestViewModels.Add(new ShortUserViewModel
                {
                    Id = user.UserId,
                    Name = user.User.FullName,
                    Avatar = user.User.Avatar
                });
            }
            return requestViewModels;
        }
        public async Task<List<ShortUserViewModel>> GetMembers(Guid id)
        {
            var users = _context.Member.Include(x => x.User).Where(x => x.GroupId == id);
            List<ShortUserViewModel> membersViewModels = new List<ShortUserViewModel>();
            foreach(var user in users)
            {
                membersViewModels.Add(new ShortUserViewModel
                {
                    Id = user.UserId,
                    Name = user.User.FullName,
                    Avatar = user.User.Avatar
                });
            }
            return membersViewModels;
        }

        public async Task<List<Review>> GetReviews(Guid id)
        {
            return await _context.Review.Include(x => x.Owner).Where(x => x.EventId == id).ToListAsync();
        }
        private async Task<Event> Get(Guid? id)
        {
            var ev = await _context.Event.FirstOrDefaultAsync(x => x.Id == id);
            if (ev == null)
            {
                throw new KeyNotFoundException($"Event with Id={id} does not found!");
            }
            return ev;
        }

        public async Task<List<ShortEventViewModel>> GetAllEvent(DateTime? data)
        {
            List<ShortEventViewModel> ev;
            if (data == null)
            {
                ev = await _context.Event.Select(x => new ShortEventViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster
                }).ToListAsync();
            }
            else
            {
                var events = _context.Event.Where(x => x.Date == data);

                ev = await events.Select(x => new ShortEventViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster
                }).ToListAsync();
            }

            return ev;
        }



        public async Task<CreateEventViewModel> GetEventView(Guid id)
        {
            var ev = await Get(id);
            return new CreateEventViewModel
            {
                Description = ev.Description,
                Title = ev.Title,
                Id = id,
                Date = ev.Date,
                Price = ev.Price
            };
        }

        public async Task<List<ShortUserViewModel>> GetSearching(Guid eventId)
        {
            var searchings = await _context.Searching.Where(x => x.EventId == eventId).ToListAsync();
            List<ShortUserViewModel> people = new List<ShortUserViewModel>();
            foreach (var search in searchings)
            {
                var human = await _context.Users.FirstOrDefaultAsync(x => x.Id == search.UserId);
                people.Add(new ShortUserViewModel
                {
                    Id = human.Id,
                    Name = human.FullName,
                    Avatar = human.Avatar,
                });
            }
            return people;
        }
        public async Task FindCompany(string email, Guid id)
        {
            var user = await GetUser(email);
            await _context.Searching.AddAsync(new Searching
            {
                UserId = user.Id,
                EventId = id,
            });
            await _context.SaveChangesAsync();
        }
        public async Task BuyTicket(string email, Guid id)
        {
            var user = await GetUser(email);
            await _context.UsersEvents.AddAsync(new UsersEvents
            {
                UserId = user.Id,
                EventId = id,
            });
            await _context.SaveChangesAsync();
        }

        private async Task<User> GetUser(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
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

        public async Task Rate(Guid id, int grade)
        {
            var ev = await Get(id);
            ev.Grade += grade;
            await _context.SaveChangesAsync();
        }

       public async Task PostReview(Guid id, string text, string email)
        {
            var review = await _context.Review.FirstOrDefaultAsync(x => x.EventId == id);

            review = new Review
            {
                EventId = id,
                Text = text,
                Date = DateTime.Now,
                Owner = await GetUser(email)
            };
            await _context.AddAsync(review);
            await _context.SaveChangesAsync();
        }
    }
}
