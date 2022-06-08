using Microsoft.AspNetCore.Identity;

namespace Backend3.Storage
{
    public static class ConfigureIdentity
    {
        public static async Task ConfigureIdentityAsync(this WebApplication app) // ключевое слово this в аргументе метода обозначает, что это будет метод расширения
        {
            using var serviceScope = app.Services.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>(); // регистрация пользователей проходит при помощи UserManager
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>(); // создание ролей происходит через RoleManager
            var config = app.Configuration.GetSection("DefaultUsersConfig");
            var adminRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Administrator); // Пытаемся найти роль админа
            if (adminRole == null) // Если ее еще нет в БД, создаем
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = ApplicationRoleNames.Administrator,
                    Type = RoleType.Administrator
                });
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {ApplicationRoleNames.Administrator} role.");
                }
                adminRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Administrator); // После создания получаем роль еще раз
            }
           
            var userRole = await roleManager.FindByNameAsync(ApplicationRoleNames.User);
            if (userRole == null) // создание роли обычного пользователя
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = ApplicationRoleNames.User,
                    Type = RoleType.User
                });
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {ApplicationRoleNames.User} role.");
                }
            }
            var organizerRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Organizer);
            if (organizerRole == null) // создание роли обычного пользователя
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = ApplicationRoleNames.Organizer,
                    Type = RoleType.Organizer
                });
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {ApplicationRoleNames.Organizer} role.");
                }
            }
        }

    }
}
