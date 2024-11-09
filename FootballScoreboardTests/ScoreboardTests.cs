using FootballScoreboard.Exceptions;
using FootballScoreboard.Interfaces;
using FootballScoreboard.Models;
using Moq;
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
        _matchRepositoryMock.Setup(x => x.GetAllActive()).Returns([new Match("Mexico", "Canada", matchStartTime)]);
        var exception = Assert.Throws<ScoreboardException>(() => _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime));
        Assert.Equal("Match validation failed: Team Mexico is already in a match.", exception.Message);
        _matchRepositoryMock.Verify(r => r.Add(It.Is<Match>(m => m.HomeTeam == homeTeam && m.AwayTeam == awayTeam)), Times.Never);
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
    public void UpdateScore_ShouldThrowException_WhenHomeTeamScoreIsNegative(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(homeTeam, awayTeam, matchStartTime);
        _matchRepositoryMock.Setup(x => x.GetSingle(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(match);

        var exception = Assert.Throws<ScoreboardException>(() => _scoreboard.UpdateScore(homeTeam, awayTeam, homeTeamScore, awayTeamScore));
        Assert.Equal("Match validation failed: Home team score cannot be negative.", exception.Message);
    }

    [Theory]
    [InlineData("Mexico", "Canada", 1, -5)]
    public void UpdateScore_ShouldThrowException_WhenAwayTeamScoreIsNegative(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(homeTeam, awayTeam, matchStartTime);
        _matchRepositoryMock.Setup(x => x.GetSingle(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(match);

        var exception = Assert.Throws<ScoreboardException>(() => _scoreboard.UpdateScore(homeTeam, awayTeam, homeTeamScore, awayTeamScore));
        Assert.Equal("Match validation failed: Away team score cannot be negative.", exception.Message);
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
        _scoreboard.GetMatches();
        _matchRepositoryMock.Verify(r => r.GetAllActive(), Times.Once);
    }
}