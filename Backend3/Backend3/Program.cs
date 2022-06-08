using Backend3.Services;
using Backend3.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, Role>() // ƒобавление identity к проекту
    .AddEntityFrameworkStores<ApplicationDbContext>() // указание контекста
    .AddSignInManager<SignInManager<User>>() // €вное указание того, что менеджер авторизации должен работать с переопределенной моделью пользовател€
    .AddUserManager<UserManager<User>>() // аналогично дл€ менеджера юзеров
    .AddRoleManager<RoleManager<Role>>(); // аналогично дл€ менеджера ролей
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();


builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUsersService, UsersService>();

var app = builder.Build();



using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
context.Database.Migrate();
await app.ConfigureIdentityAsync();

/*if (context.Roles.FirstOrDefault(x => x.Type == RoleType.User) == default)
{
    var role = new Role
    {
        Type = RoleType.User
    };
    context.Add(role);
    context.SaveChanges();
}
if (context.Roles.FirstOrDefault(x => x.Type == RoleType.Organizer) == default)
{
    var role = new Role
    {
        Type = RoleType.Organizer
    };
    context.Add(role);
    context.SaveChanges();
}
if (context.Roles.FirstOrDefault(x => x.Type == RoleType.Administrator) == default)
{
    var role = new Role
    {
        Type = RoleType.Administrator
    };
    context.Add(role);
    context.SaveChanges();
}*/

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Event}/{action=Index}/{id?}");

app.Run();
