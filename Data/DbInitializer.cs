using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.Models;

namespace MovieSeriesCatalog.Data;

public static class DbInitializer
{
    private static readonly string[] RequiredTables =
    {
        "__EFMigrationsHistory",
        "AspNetRoles",
        "AspNetUsers",
        "Directors",
        "Actors",
        "Movies",
        "MovieActors",
        "Reviews"
    };

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.MigrateAsync();
        await RebuildDatabaseIfSchemaIsIncompleteAsync(context);

        await EnsureRolesAsync(roleManager);

        var adminUser = await EnsureUserAsync(
            userManager,
            "admin@movieseriescatalog.local",
            "Admin123!",
            RoleNames.Admin);

        var demoUser = await EnsureUserAsync(
            userManager,
            "user@movieseriescatalog.local",
            "User123!",
            RoleNames.User);

        if (!await context.Directors.AnyAsync())
        {
            var directors = new[]
            {
                new Director("Christopher Nolan", "Award-winning director known for science-fiction and historical dramas."),
                new Director("Martin Scorsese", "Director celebrated for character-driven crime stories and ambitious filmmaking.")
            };

            context.Directors.AddRange(directors);
            await context.SaveChangesAsync();
        }

        if (!await context.Actors.AnyAsync())
        {
            var actors = new[]
            {
                new Actor("Leonardo DiCaprio", "Versatile actor known for psychological drama and large-scale productions."),
                new Actor("Cillian Murphy", "Irish actor recognized for intense dramatic performances."),
                new Actor("Matthew McConaughey", "Actor known for emotionally grounded lead performances.")
            };

            context.Actors.AddRange(actors);
            await context.SaveChangesAsync();
        }

        if (!await context.Movies.AnyAsync())
        {
            var directors = await context.Directors.OrderBy(director => director.FullName).ToListAsync();
            var actors = await context.Actors.OrderBy(actor => actor.FullName).ToListAsync();

            var nolan = directors.Single(director => director.FullName == "Christopher Nolan");
            var scorsese = directors.Single(director => director.FullName == "Martin Scorsese");

            var cillianMurphy = actors.Single(actor => actor.FullName == "Cillian Murphy");
            var leonardoDiCaprio = actors.Single(actor => actor.FullName == "Leonardo DiCaprio");
            var matthewMcConaughey = actors.Single(actor => actor.FullName == "Matthew McConaughey");

            var movies = new[]
            {
                new Movie(
                    CatalogType.Movie,
                    "Inception",
                    "A skilled thief enters dreams to steal and plant ideas while struggling with his own guilt.",
                    2010,
                    GenreType.SciFi,
                    nolan.Id),
                new Movie(
                    CatalogType.Movie,
                    "Interstellar",
                    "A group of astronauts search for a new home for humanity through unstable wormholes.",
                    2014,
                    GenreType.SciFi,
                    nolan.Id),
                new Movie(
                    CatalogType.Movie,
                    "Oppenheimer",
                    "The story of the theoretical physicist behind the Manhattan Project and its consequences.",
                    2023,
                    GenreType.Drama,
                    nolan.Id),
                new Movie(
                    CatalogType.Movie,
                    "The Departed",
                    "An undercover officer and a mole race to expose each other inside rival organizations.",
                    2006,
                    GenreType.Crime,
                    scorsese.Id),
                new Movie(
                    CatalogType.Movie,
                    "Shutter Island",
                    "A U.S. Marshal investigates a disturbing disappearance at a remote psychiatric institution.",
                    2010,
                    GenreType.Thriller,
                    scorsese.Id)
            };

            movies[0].SetActors(new[] { leonardoDiCaprio.Id, cillianMurphy.Id });
            movies[1].SetActors(new[] { matthewMcConaughey.Id, cillianMurphy.Id });
            movies[2].SetActors(new[] { cillianMurphy.Id, leonardoDiCaprio.Id });
            movies[3].SetActors(new[] { leonardoDiCaprio.Id, matthewMcConaughey.Id });
            movies[4].SetActors(new[] { leonardoDiCaprio.Id, cillianMurphy.Id });

            context.Movies.AddRange(movies);
            await context.SaveChangesAsync();
        }

        if (!await context.Reviews.AnyAsync())
        {
            var movies = await context.Movies.OrderBy(movie => movie.Title).ToListAsync();

            var inception = movies.Single(movie => movie.Title == "Inception");
            var interstellar = movies.Single(movie => movie.Title == "Interstellar");
            var oppenheimer = movies.Single(movie => movie.Title == "Oppenheimer");
            var theDeparted = movies.Single(movie => movie.Title == "The Departed");
            var shutterIsland = movies.Single(movie => movie.Title == "Shutter Island");

            var reviews = new[]
            {
                new Review(inception.Id, adminUser.Id, "A layered science-fiction story with excellent pacing and a memorable ending.", 10),
                new Review(inception.Id, demoUser.Id, "Inventive, stylish, and easy to revisit because every scene adds another clue.", 9),
                new Review(interstellar.Id, adminUser.Id, "Ambitious world building and strong emotional stakes throughout the mission.", 9),
                new Review(oppenheimer.Id, demoUser.Id, "Dense but rewarding historical drama carried by a powerful lead performance.", 9),
                new Review(theDeparted.Id, adminUser.Id, "A tense crime thriller with great character work and relentless momentum.", 8),
                new Review(shutterIsland.Id, demoUser.Id, "Atmospheric and unsettling from the first act to the final reveal.", 8),
                new Review(shutterIsland.Id, adminUser.Id, "Strong suspense, excellent performances, and a satisfying payoff.", 9)
            };

            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();
        }
    }

    private static async Task RebuildDatabaseIfSchemaIsIncompleteAsync(ApplicationDbContext context)
    {
        var missingTables = await GetMissingTablesAsync(context);
        if (missingTables.Count == 0)
        {
            return;
        }

        await context.Database.CloseConnectionAsync();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    private static async Task<List<string>> GetMissingTablesAsync(ApplicationDbContext context)
    {
        var connection = context.Database.GetDbConnection();
        var openedHere = false;

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
            openedHere = true;
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table';";

        var existingTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            existingTables.Add(reader.GetString(0));
        }

        if (openedHere)
        {
            await connection.CloseAsync();
        }

        return RequiredTables
            .Where(tableName => !existingTables.Contains(tableName))
            .ToList();
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in new[] { RoleNames.Admin, RoleNames.User })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string roleName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Unable to seed user '{email}': {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }

        return user;
    }
}
