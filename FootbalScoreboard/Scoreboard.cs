namespace FootbalScoreboard;

public class Scoreboard
{
    private List<Match> _matches = [];

    public void StartMatch(string homeTeam, string awayTeam, DateTime matchStartTime)
    {
        Match match = new(homeTeam, awayTeam, matchStartTime);
        _matches.Add(match);
    }

    public void UpdateScore(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
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
        return _matches
            .OrderByDescending(x => x.HomeTeamScore + x.AwayTeamScore)
            .ThenByDescending(x => x.StartTime)
            .ToList();
    }
}
