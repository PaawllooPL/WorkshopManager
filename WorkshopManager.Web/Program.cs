using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.DAL.EF;
using WorkshopManager.Model.DataModels;
using WorkshopManager.Services;
using System.Net.Mail;
using System.Net;
using WorkshopManager.Web.Models;

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

builder.Services.AddScoped<StatusDescriptionService>();
builder.Services.AddTransient(typeof(ILogger), typeof(Logger<Program>));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<SmtpSettings>();
builder.Services.AddScoped<EmailNotificationService>();



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
        var smtpSettings = scope.ServiceProvider.GetService<SmtpSettings>()!;

        DotNetEnv.Env.TraversePath().Load();
        var ownerEmail = System.Environment.GetEnvironmentVariable("OWNER_EMAIL");
        var ownerPassword = System.Environment.GetEnvironmentVariable("OWNER_PASSWORD");
        var ownerFirstName = System.Environment.GetEnvironmentVariable("OWNER_FIRSTNAME");
        var ownerLastName = System.Environment.GetEnvironmentVariable("OWNER_LASTNAME");
        
        var smtpEmail = System.Environment.GetEnvironmentVariable("SMTP_EMAIL");
        var smtpPassword = System.Environment.GetEnvironmentVariable("SMTP_PASSWORD");
        var smtpHost = System.Environment.GetEnvironmentVariable("SMTP_HOST");
        var smtpPort = System.Environment.GetEnvironmentVariable("SMTP_PORT");

        if (String.IsNullOrEmpty(ownerEmail))
            throw new Exception("Environmental variable is missing owner email");
        if (String.IsNullOrEmpty(ownerPassword))
            throw new Exception("Environmental variable is missing owner password");
        if (String.IsNullOrEmpty(ownerFirstName))
            throw new Exception("Environmental variable is missing owner first name");
        if (String.IsNullOrEmpty(ownerLastName))
            throw new Exception("Environmental variable is missing owner last name");

        if(smtpPort != null)
        {
            if(ValidateSmtpCredentials(smtpHost ?? "", int.Parse(smtpPort), smtpEmail ?? "", smtpPassword ?? ""))
            {
                smtpSettings.host = smtpHost!;
                smtpSettings.port = int.Parse(smtpPort);
                smtpSettings.email = smtpEmail!;
                smtpSettings.password = smtpPassword!;
                smtpSettings.isActive = true;
            }
        }
        
        var checkCurrentAdmin = await userManager.FindByEmailAsync(ownerEmail);
        

        if (checkCurrentAdmin != null)
        {
            if(checkCurrentAdmin.PasswordHash != userManager.PasswordHasher.HashPassword(checkCurrentAdmin, ownerPassword))
            {
                try
                {
                    await userManager.RemovePasswordAsync(checkCurrentAdmin);
                    await userManager.AddPasswordAsync(checkCurrentAdmin, ownerPassword);
                } 
                catch (Exception ex)
                {
                    throw new Exception("Could not update owner password.");
                }

            }
            return;
        }

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

bool ValidateSmtpCredentials(string smtpHost, int smtpPort, string email, string password)
{
    try
    {
        using (var client = new SmtpClient(smtpHost, smtpPort))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(email, password);
            client.Timeout = 10000;

            client.Send(new MailMessage(email, email, "Test SMTP", "Test po³¹czenia SMTP. " + DateTime.Now.ToString()));
        }

        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"B³¹d SMTP: {ex.Message}");
        return false;
    }
}
