namespace MovieSeriesCatalog.Models;

public class MovieActor : BaseEntity
{
    private MovieActor()
    {
    }

    public MovieActor(int actorId)
    {
        ActorId = actorId;
    }

    public int MovieId { get; private set; }

    public Movie? Movie { get; private set; }

    public int ActorId { get; private set; }

    public Actor? Actor { get; private set; }
}
