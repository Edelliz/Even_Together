using Backend3.Models;
using Backend3.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Backend3.Services
{
    public interface IUsersService
    {
        Task Register(RegisterViewModel model);
        Task Logout();
        Task Login(LoginViewModel model);
    }

    public class UsersService : IUsersService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly RoleManager<Role> _roleManager;

        private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };
        public UsersService(UserManager<User> userManager,
                SignInManager<User> signInManager, ApplicationDbContext context, IWebHostEnvironment environment, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _environment = environment;
            _roleManager = roleManager;
        }

        public async Task Register(RegisterViewModel model)
        {

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                BirthDate = model.BirthDate,
                FullName = model.Name,
                Avatar = await AddFile(model.Avatar)
            };

            var role = _context.Roles.FirstOrDefault(x => x.Type == RoleType.User);
            if (model.IsOrganizer)
            {
                role = _context.Roles.FirstOrDefault(x => x.Type == RoleType.Organizer);
            }

            var result = await _userManager.CreateAsync(user, model.Password); // Создание нового пользователя в системе с указанными данными и введенным паролем
            if (result.Succeeded) // результат может быть успешным, может также возникнуть ошибка, если был введен пароль, не отвечающий требованиям
            {
                await _userManager.AddToRoleAsync(user, role.Name); // назначение на роль происходит через сущность пользователя и название роли (строка). Если при обычной процедуре регистрации нужно назначить роль обычного пользователя к новому зарегистрированному, необходимо будет после регистрации использовать данный метод для назначения пользователя на роль.
                // Если регистрация прошла успешно, авторизуем пользователя в системе. Следующая строка создает cookie, который будет использоватся в следующих запросах от пользователя
                await _signInManager.SignInAsync(user, false);
                return;
            }
            // Если произошла ошибка, собираем все ошибки в одну строку и выбрасываем наверх исключение
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new ArgumentException(errors);
        }
        public async Task Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email); // пытаемся найти юзера по email
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email = {model.Email} does not found");
            }

            // Далее генерируем набор клеймов, состоящих из необходимых для быстрого доступа данных
            var claims = new List<Claim>
            {
                new ("UserName", user.UserName),
                 new ("Id", user.Id.ToString()),
                new (ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Также в клеймы добавляем все роли пользователя, если они есть
            if (user.Roles?.Any() == true)
            {
                var roles = user.Roles.Select(x => x.Role).ToList();
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
            }

            // Задаем параметры аутентификации
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2), // Куки будет жить 2 дня
                IsPersistent = true
            };

            // Процесс авторизации и создания куки
            await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);
        }

        public async Task Logout()
        {
            // Выход из системы == удаление куки
            await _signInManager.SignOutAsync();
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
