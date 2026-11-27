using ClassLib.Model;
using ClassLib.Repo;
using ClassLib.Interface;
using ClassLib.Model;
using ClassLib.Repo;
using ClassLib.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace BorumProjektstyring
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

    

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<BorumBygDBContext>();

            builder.Services.AddScoped<ProjectRepo>();
            builder.Services.AddScoped<UserRepo>();
            IUserAuthRepo userRepo = new UserAuthRepo();
            // We are using scoped because we want a new instance of the repository for each HTTP request.
            // This is important for managing database connections and ensuring that each request has its own context.
            builder.Services.AddScoped<IUserAuthRepo, UserAuthRepo>();
            builder.Services.AddScoped<UserService>();

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login"; // Set the login page path
                    options.AccessDeniedPath = "/AccessDenied"; // Set the access denied page path
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Koordinator", policy => policy.RequireRole("Koordinator"));
                options.AddPolicy("Bruger", policy => policy.RequireRole("Bruger"));
                options.AddPolicy("Profile", policy =>
                    policy.RequireRole("Koordinator", "Bruger"));
            });

            builder.Services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeFolder("/Admin", "Admin");
                options.Conventions.AuthorizeFolder("/Profile", "Profile");
            });

            



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                var mustChangePassword =
                    context.User.Identity?.IsAuthenticated == true &&
                    context.User.HasClaim("MustChangePassword", "true");

                var path = context.Request.Path;
                var isAllowedPath =
                    path.StartsWithSegments("/ChangePassword") ||
                    path.StartsWithSegments("/Login") ||
                    path.StartsWithSegments("/Logout") ||
                    path.StartsWithSegments("/css") ||
                    path.StartsWithSegments("/js") ||
                    path.StartsWithSegments("/lib");

                if (mustChangePassword && !isAllowedPath)
                {
                    context.Response.Redirect("/ChangePassword");
                    return;
                }

                await next();
            });

            app.UseAuthorization();
            

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
