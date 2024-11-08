using FootballScoreboard.Models;

namespace FootballScoreboard.Interfaces;
public interface IScoreboard
{
    void StartMatch(string homeTeam, string awayTeam, DateTime matchStartTime);
    void UpdateScore(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore);
    void FinishMatch(string homeTeam, string awayTeam);
    List<Match> GetMatches();
}
