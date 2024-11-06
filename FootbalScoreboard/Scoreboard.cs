namespace FootbalScoreboard;

public class Scoreboard
{
    private List<Match> _matches = [];

    public void StartMatch(string homeTeam, string awayTeam, DateTime matchStartTime)
    {
        Match match = new(homeTeam, awayTeam, matchStartTime);
        _matches.Add(match);
    }

    public List<Match> GetMatches()
    {
        return _matches;
    }
}
