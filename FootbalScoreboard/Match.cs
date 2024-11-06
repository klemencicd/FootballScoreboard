namespace FootbalScoreboard;
public class Match(string homeTeam, string awayTeam)
{
    public string HomeTeam { get; private set; } = homeTeam;
    public string AwayTeam { get; private set; } = awayTeam;
    public int HomeTeamScore { get; private set; } = 0;
    public int AwayTeamScore { get; private set; } = 0;
    public DateTime StartTime { get; set; } = DateTime.Now;
}
