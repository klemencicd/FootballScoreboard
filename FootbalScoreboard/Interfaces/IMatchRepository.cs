namespace FootbalScoreboard.Interfaces;
public interface IMatchRepository
{
    void Add(Match match);
    void Remove(Match match);
    Match? GetSingle(string homeTeam, string awayTeam);
    List<Match> GetAllActive();
}
