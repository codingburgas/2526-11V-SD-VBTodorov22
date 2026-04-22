using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieSeriesCatalog.Models;

namespace MovieSeriesCatalog.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<Director> Directors => Set<Director>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieActor> MovieActors => Set<MovieActor>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Actor>(entity =>
        {
            entity.Property(actor => actor.FullName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(actor => actor.Biography)
                .HasMaxLength(1000)
                .IsRequired();
        });

        builder.Entity<Director>(entity =>
        {
            entity.Property(director => director.FullName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(director => director.Biography)
                .HasMaxLength(1000)
                .IsRequired();
        });

        builder.Entity<Movie>(entity =>
        {
            entity.Property(movie => movie.Title)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(movie => movie.Description)
                .HasMaxLength(2000)
                .IsRequired();

            entity.HasOne(movie => movie.Director)
                .WithMany(director => director.Movies)
                .HasForeignKey(movie => movie.DirectorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MovieActor>(entity =>
        {
            entity.HasIndex(movieActor => new { movieActor.MovieId, movieActor.ActorId })
                .IsUnique();

            entity.HasOne(movieActor => movieActor.Movie)
                .WithMany(movie => movie.MovieActors)
                .HasForeignKey(movieActor => movieActor.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(movieActor => movieActor.Actor)
                .WithMany(actor => actor.MovieActors)
                .HasForeignKey(movieActor => movieActor.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Review>(entity =>
        {
            entity.Property(review => review.Comment)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasOne(review => review.Movie)
                .WithMany(movie => movie.Reviews)
                .HasForeignKey(review => review.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(review => review.User)
                .WithMany(user => user.Reviews)
                .HasForeignKey(review => review.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
