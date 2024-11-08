using FootbalScoreboard;
using FootbalScoreboard.Exceptions;
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
    [InlineData("Mexico", "Italy")]
    public void StartMatch_ShouldThrowExceptionIfTeamIsAlreadyPlaying(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime);
        var exception = Assert.Throws<ScoreboardException>(() => _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime));
        Assert.Equal("Team Mexico cannot start this match because they are already playing.", exception.Message);
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

    [Theory]
    [InlineData("Mexico", "Canada", -1, 5)]
    public void UpdateScore_ShouldThrowExceptionIfScoreIsNegativeNumber(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime);
        var exception = Assert.Throws<ScoreboardException>(() => _scoreboard.UpdateScore(homeTeam, awayTeam, homeTeamScore, awayTeamScore));
        Assert.Equal("Score cannot be negative number.", exception.Message);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void FinishMatch_ShouldRemoveMatchFromScoreboard(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime);
        _scoreboard.FinishMatch(homeTeam, awayTeam);
        List<Match> matches = _scoreboard.GetMatches();

        Assert.Empty(matches);
    }

    [Fact]
    public void GetMatches_ShouldReturnMatchesOrderedByTotalScoreThenByStartTime()
    {
        var matchStartTime = DateTime.UtcNow;
        _scoreboard.StartMatch("Mexico", "Canada", matchStartTime);
        _scoreboard.UpdateScore("Mexico", "Canada", 0, 5);
        _scoreboard.StartMatch("Spain", "Brazil", matchStartTime.AddMinutes(1));
        _scoreboard.UpdateScore("Spain", "Brazil", 10, 2);
        _scoreboard.StartMatch("Germany", "France", matchStartTime.AddMinutes(2));
        _scoreboard.UpdateScore("Germany", "France", 2, 2);
        _scoreboard.StartMatch("Uruguay", "Italy", matchStartTime.AddMinutes(3));
        _scoreboard.UpdateScore("Uruguay", "Italy", 6, 6);
        _scoreboard.StartMatch("Argentina", "Australia", matchStartTime.AddMinutes(4));
        _scoreboard.UpdateScore("Argentina", "Australia", 3, 1);

        List<Match> matches = _scoreboard.GetMatches();

        Assert.Equal("Uruguay", matches[0].HomeTeam);
        Assert.Equal("Italy", matches[0].AwayTeam);
        Assert.Equal("Spain", matches[1].HomeTeam);
        Assert.Equal("Brazil", matches[1].AwayTeam);
        Assert.Equal("Mexico", matches[2].HomeTeam);
        Assert.Equal("Canada", matches[2].AwayTeam);
        Assert.Equal("Argentina", matches[3].HomeTeam);
        Assert.Equal("Australia", matches[3].AwayTeam);
        Assert.Equal("Germany", matches[4].HomeTeam);
        Assert.Equal("France", matches[4].AwayTeam);
    }
}