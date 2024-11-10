using FootballScoreboard.Models;

namespace FootballScoreboard.Interfaces;
public interface IScoreboard
{
    Ulid StartMatch(string homeTeam, string awayTeam, DateTime matchStartTime);
    void UpdateScore(Ulid id, int homeTeamScore, int awayTeamScore);
    void FinishMatch(Ulid id);
    List<Match> GetMatches();
}
