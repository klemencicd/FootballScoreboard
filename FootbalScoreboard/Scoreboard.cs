using FootbalScoreboard.Exceptions;
using FootbalScoreboard.Interfaces;

namespace FootbalScoreboard;

public class Scoreboard(IMatchRepository _matchRepository) : IScoreboard
{
    private readonly List<Match> _matches = _matchRepository.GetAllActiveMatches();

    public void StartMatch(string homeTeam, string awayTeam, DateTime matchStartTime)
    {
        if (_matches.Any(x => x.HomeTeam.Equals(homeTeam) || x.AwayTeam.Equals(homeTeam)))
            throw new ScoreboardException($"Team {homeTeam} cannot start this match because they are already playing.");

        if (_matches.Any(x => x.HomeTeam.Equals(awayTeam) || x.AwayTeam.Equals(awayTeam)))
            throw new ScoreboardException($"Team {awayTeam} cannot start this match because they are already playing.");

        Match match = new(homeTeam, awayTeam, matchStartTime);
        _matches.Add(match);
    }

    public void UpdateScore(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        if (homeTeamScore < 0 || awayTeamScore < 0)
            throw new ScoreboardException("Score cannot be negative number.");

        Match? match = _matches.SingleOrDefault(x => x.HomeTeam.Equals(homeTeam) && x.AwayTeam.Equals(awayTeam));
        match?.UpdateScore(homeTeamScore, awayTeamScore);
    }

    public void FinishMatch(string homeTeam, string awayTeam)
    {
        Match? match = _matches.SingleOrDefault(x => x.HomeTeam.Equals(homeTeam) && x.AwayTeam.Equals(awayTeam));
        if (match != null)
        {
            _matches.Remove(match);
        }
    }

    public List<Match> GetMatches()
    {
        return [.. _matches
            .OrderByDescending(x => x.HomeTeamScore + x.AwayTeamScore)
            .ThenByDescending(x => x.StartTime)];
    }
}
