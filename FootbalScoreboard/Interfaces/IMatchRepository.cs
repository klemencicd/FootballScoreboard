namespace FootbalScoreboard.Interfaces;
public interface IMatchRepository
{
    void AddMatch(Match match);
    void RemoveMatch(Match match);
    Match GetMatch(string homeTeam, string awayTeam);
    List<Match> GetAllActiveMatches();
}
