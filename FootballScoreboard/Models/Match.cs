namespace FootballScoreboard.Models;
public class Match(Ulid id, string homeTeam, string awayTeam, DateTime startTime)
{
    public Ulid Id { get; set; } = id;
    public string HomeTeam { get; private set; } = homeTeam;
    public string AwayTeam { get; private set; } = awayTeam;
    public int HomeTeamScore { get; private set; } = 0;
    public int AwayTeamScore { get; private set; } = 0;
    public DateTime StartTime { get; private set; } = startTime;
    public int TotalScore => HomeTeamScore + AwayTeamScore;

    internal void UpdateScore(int homeTeamScore, int awayTeamScore)
    {
        HomeTeamScore = homeTeamScore;
        AwayTeamScore = awayTeamScore;
    }
}
