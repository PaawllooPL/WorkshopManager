using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using DotNetEnv;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityCore<Client>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<Role>()
                .AddSignInManager<SignInManager<Client>>()
                .AddUserManager<UserManager<Client>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityCore<Owner>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<Role>()
                .AddSignInManager<SignInManager<Owner>>()
                .AddUserManager<UserManager<Owner>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient(typeof(ILogger), typeof(Logger<Program>));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

await SeedOwnerAccount(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();



 async Task SeedOwnerAccount (IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetService<UserManager<Owner>>()!;
        var roleManager = scope.ServiceProvider.GetService<RoleManager<Role>>()!;

        DotNetEnv.Env.TraversePath().Load();
        var ownerEmail = System.Environment.GetEnvironmentVariable("OWNER_EMAIL");
        var ownerPassword = System.Environment.GetEnvironmentVariable("OWNER_PASSWORD");
        var ownerFirstName = System.Environment.GetEnvironmentVariable("OWNER_FIRSTNAME");
        var ownerLastName = System.Environment.GetEnvironmentVariable("OWNER_LASTNAME");

        if (String.IsNullOrEmpty(ownerEmail))
            throw new Exception("Environmental variable is missing owner email");
        if (String.IsNullOrEmpty(ownerPassword))
            throw new Exception("Environmental variable is missing owner password");
        if (String.IsNullOrEmpty(ownerFirstName))
            throw new Exception("Environmental variable is missing owner first name");
        if (String.IsNullOrEmpty(ownerLastName))
            throw new Exception("Environmental variable is missing owner last name");

        var checkCurrentAdmin = await userManager.FindByEmailAsync(ownerEmail);

        if (checkCurrentAdmin != null)
            return;

        var owner = Activator.CreateInstance<Owner>();
        owner.FirstName = ownerFirstName;
        owner.LastName = ownerLastName;
        owner.UserName = ownerEmail;
        owner.Email = ownerEmail;

        var result = userManager.CreateAsync(owner, ownerPassword).Result;

        if(result.Succeeded)
        {
            await userManager.SetEmailAsync(owner, ownerEmail);
            await userManager.SetUserNameAsync(owner, ownerEmail);
            await userManager.AddToRoleAsync(owner, RoleValue.Owner.ToString());
            return;
        }

        throw new Exception("Seeding owner account went wrong");
    }
}
