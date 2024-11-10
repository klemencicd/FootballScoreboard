using FootballScoreboard.Models;

namespace FootballScoreboard.Interfaces;
public interface IMatchRepository
{
    void Add(Match match);
    void Remove(Match match);
    Match? GetSingle(Ulid id);
    List<Match> GetAllActive();
}
