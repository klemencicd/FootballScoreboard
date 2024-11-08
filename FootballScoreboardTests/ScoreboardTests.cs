using FootballScoreboard.Exceptions;
using FootballScoreboard.Interfaces;
using FootballScoreboard.Models;
using Moq;
using System.Text.RegularExpressions;
using Match = FootballScoreboard.Models.Match;

namespace FootballScoreboardTests;

public class ScoreboardTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock = new();
    private readonly Scoreboard _scoreboard;

    public ScoreboardTests()
    {
        _scoreboard = new(_matchRepositoryMock.Object);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void StartMatch_ShouldStartNewMatch_WithInitialScoreZero(string homeTeam, string awayTeam)
    {
        _matchRepositoryMock.Setup(x => x.GetAllActive()).Returns([]);

        var matchStartTime = DateTime.UtcNow;
        _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime);

        _matchRepositoryMock.Verify(r => r.Add(It.Is<Match>(m => m.HomeTeam == homeTeam && m.AwayTeam == awayTeam)), Times.Once);
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
    [InlineData("Mexico", "Canada", 1, 5)]
    public void UpdateScore_ShouldUpdateMatchScore(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(homeTeam, awayTeam, matchStartTime);
        _matchRepositoryMock.Setup(x => x.GetSingle(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(match);

        _scoreboard.UpdateScore(homeTeam, awayTeam, homeTeamScore, awayTeamScore);

        Assert.Equal(homeTeam, match.HomeTeam);
        Assert.Equal(1, match.HomeTeamScore);
        Assert.Equal(awayTeam, match.AwayTeam);
        Assert.Equal(5, match.AwayTeamScore);
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
        Match match = new(homeTeam, awayTeam, matchStartTime);
        _matchRepositoryMock.Setup(x => x.GetSingle(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(match);
        _scoreboard.FinishMatch(homeTeam, awayTeam);
        _matchRepositoryMock.Verify(r => r.Remove(match), Times.Once);
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