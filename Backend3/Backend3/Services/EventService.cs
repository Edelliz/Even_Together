using Backend3.Models;
using Backend3.Storage;
using Microsoft.EntityFrameworkCore;

namespace Backend3.Services
{
    public interface IEventService
    {
        Task CreateEvent(CreateEventViewModel model, string email);
        Task ChangeEvent(CreateEventViewModel model);
        Task<List<ShortEventViewModel>> GetAllEvent();
        Task<CreateEventViewModel> GetEventView(Guid id);
        Task FindCompany(string email, Guid id);
        Task BuyTicket(string email, Guid id);
        Task<List<ShortUserViewModel>> GetSearching(Guid eventId);
        Task<EventViewModel> GetEvent(Guid id, string userEmail);
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
            if(ev.Organizer == userEmail)
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
                Reviews = await GetReviews(id)
            };
        }
        public async Task<List<Review>> GetReviews(Guid id)
        {
            return await _context.Review.Where(x => x.EventId == id).ToListAsync();
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

        public async Task<List<ShortEventViewModel>> GetAllEvent()
        {
            var ev = await _context.Event.Select(x => new ShortEventViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Poster = x.Poster
            }).ToListAsync();
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
            foreach(var search in searchings)
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
    }
}
