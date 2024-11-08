namespace FootballScoreboard;
public class Match(string homeTeam, string awayTeam, DateTime startTime)
{
    public string HomeTeam { get; private set; } = homeTeam;
    public string AwayTeam { get; private set; } = awayTeam;
    public int HomeTeamScore { get; private set; } = 0;
    public int AwayTeamScore { get; private set; } = 0;
    public DateTime StartTime { get; private set; } = startTime;

    internal void UpdateScore(int homeTeamScore, int awayTeamScore)
    {
        HomeTeamScore = homeTeamScore;
        AwayTeamScore = awayTeamScore;
    }
}
