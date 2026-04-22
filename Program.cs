using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.Data;
using MovieSeriesCatalog.Models;
using MovieSeriesCatalog.Services.Implementations;
using MovieSeriesCatalog.Services.Interfaces;

namespace MovieSeriesCatalog;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder(connectionString);
        if (!Path.IsPathRooted(sqliteConnectionStringBuilder.DataSource))
        {
            sqliteConnectionStringBuilder.DataSource = Path.Combine(
                builder.Environment.ContentRootPath,
                sqliteConnectionStringBuilder.DataSource);
        }

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(sqliteConnectionStringBuilder.ConnectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IMovieService, MovieService>();
        builder.Services.AddScoped<IActorService, ActorService>();
        builder.Services.AddScoped<IDirectorService, DirectorService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IStatisticsService, StatisticsService>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();

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

        await DbInitializer.InitializeAsync(app.Services);

        app.Run();
    }
}
