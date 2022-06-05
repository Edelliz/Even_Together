using Backend3.Models;
using Backend3.Storage;
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

        private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };
        public AccountService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<UserViewModel> GetUser(Guid id, string email)
        {
            var user = await Get(id);

            bool isOwner = false;
            if(user.FullName == email)
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
                IsOwner = isOwner
            };
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
