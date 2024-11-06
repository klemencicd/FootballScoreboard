using FootbalScoreboard;
using Xunit.Sdk;

namespace FootbalScoreboardTests;

public class ScoreboardTests()
{
    private readonly Scoreboard _scoreboard = new();

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void StartMatch_ShouldStartNewMatch_WithInitialScoreZero(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime);
        List<Match> matches = _scoreboard.GetMatches();

        Assert.Single(matches);
        Assert.Equal(homeTeam, matches[0].HomeTeam);
        Assert.Equal(0, matches[0].HomeTeamScore);
        Assert.Equal(awayTeam, matches[0].AwayTeam);
        Assert.Equal(0, matches[0].AwayTeamScore);
        Assert.Equal(matchStartTime, matches[0].StartTime);
    }

    [Theory]
    [InlineData("Mexico", "Canada", 0, 5)]
    public void UpdateScore_ShouldUpdateMatchScore(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime);
        _scoreboard.UpdateScore(homeTeam, awayTeam, homeTeamScore, awayTeamScore);
        List<Match> matches = _scoreboard.GetMatches();

        Assert.Single(matches);
        Assert.Equal(homeTeam, matches[0].HomeTeam);
        Assert.Equal(0, matches[0].HomeTeamScore);
        Assert.Equal(awayTeam, matches[0].AwayTeam);
        Assert.Equal(5, matches[0].AwayTeamScore);
    }
}