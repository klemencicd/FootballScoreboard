using FluentValidation.Results;
using FootballScoreboard.Exceptions;
using FootballScoreboard.Interfaces;
using FootballScoreboard.Models;
using Moq;
using Match = FootballScoreboard.Models.Match;

namespace FootballScoreboardTests;

public class ScoreboardTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock = new();
    private readonly Mock<IMatchValidator> _matchValidatorMock = new();
    private readonly Scoreboard _scoreboard;

    public ScoreboardTests()
    {
        _scoreboard = new(_matchRepositoryMock.Object, _matchValidatorMock.Object);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void StartMatch_ShouldStartNewMatch_WithInitialScoreZero(string homeTeam, string awayTeam)
    {
        _matchValidatorMock.Setup(x => x.ValidateStart(It.IsAny<Match>(), It.IsAny<List<Match>>())).Returns(new ValidationResult());
        _matchRepositoryMock.Setup(x => x.GetAllActive()).Returns([]);

        var matchStartTime = DateTime.UtcNow;
        Ulid id = _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime);

        Assert.NotEqual(Ulid.Empty, id);
        _matchValidatorMock.Verify(r => r.ValidateStart(It.IsAny<Match>(), It.IsAny<List<Match>>()), Times.Once);
        _matchRepositoryMock.Verify(r => r.Add(It.Is<Match>(m => m.HomeTeam == homeTeam && m.AwayTeam == awayTeam)), Times.Once);
    }

    [Theory]
    [InlineData("Mexico", "Italy")]
    public void StartMatch_ShouldThrowExceptionIfTeamIsAlreadyPlaying(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        List<ValidationFailure> failure = 
        [
            new ValidationFailure(homeTeam,"Team Mexico is already in a match.")
        ];
        _matchValidatorMock.Setup(x => x.ValidateStart(It.IsAny<Match>(), It.IsAny<List<Match>>()))
            .Returns(new ValidationResult(failure));
        _matchRepositoryMock.Setup(x => x.GetAllActive()).Returns([new Match(Ulid.NewUlid(), "Mexico", "Canada", matchStartTime)]);
        var exception = Assert.Throws<ScoreboardException>(() => _scoreboard.StartMatch(homeTeam, awayTeam, matchStartTime));
        Assert.Equal("Match validation failed: Team Mexico is already in a match.", exception.Message);
        _matchValidatorMock.Verify(r => r.ValidateStart(It.IsAny<Match>(), It.IsAny<List<Match>>()), Times.Once);
        _matchRepositoryMock.Verify(r => r.Add(It.Is<Match>(m => m.HomeTeam == homeTeam && m.AwayTeam == awayTeam)), Times.Never);
    }

    [Theory]
    [InlineData("Mexico", "Canada", 1, 5)]
    public void UpdateScore_ShouldUpdateMatchScore(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Ulid id = Ulid.NewUlid();
        Match match = new(id, homeTeam, awayTeam, matchStartTime);
        _matchRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Ulid>()))
            .Returns(match);
        _matchValidatorMock.Setup(x => x.ValidateScore(It.IsAny<Match>()))
            .Returns(new ValidationResult());

        _scoreboard.UpdateScore(id, homeTeamScore, awayTeamScore);

        Assert.Equal(homeTeam, match.HomeTeam);
        Assert.Equal(1, match.HomeTeamScore);
        Assert.Equal(awayTeam, match.AwayTeam);
        Assert.Equal(5, match.AwayTeamScore);
        _matchValidatorMock.Verify(r => r.ValidateScore(It.IsAny<Match>()), Times.Once);
        _matchRepositoryMock.Verify(r => r.GetSingle(It.IsAny<Ulid>()), Times.Once);
    }

    [Theory]
    [InlineData("Mexico", "Canada", -1, 5)]
    public void UpdateScore_ShouldThrowException_WhenHomeTeamScoreIsNegative(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Ulid id = Ulid.NewUlid();
        Match match = new(id, homeTeam, awayTeam, matchStartTime);
        _matchRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Ulid>()))
            .Returns(match);

        List<ValidationFailure> failure =
        [
            new ValidationFailure(homeTeam,"Home team score cannot be negative.")
        ];
        _matchValidatorMock.Setup(x => x.ValidateScore(It.IsAny<Match>()))
            .Returns(new ValidationResult(failure));

        var exception = Assert.Throws<ScoreboardException>(() => _scoreboard.UpdateScore(id, homeTeamScore, awayTeamScore));
        Assert.Equal("Match validation failed: Home team score cannot be negative.", exception.Message);
        _matchValidatorMock.Verify(r => r.ValidateScore(It.IsAny<Match>()), Times.Once);
        _matchRepositoryMock.Verify(r => r.GetSingle(It.IsAny<Ulid>()), Times.Once);
    }

    [Theory]
    [InlineData("Mexico", "Canada", 1, -5)]
    public void UpdateScore_ShouldThrowException_WhenAwayTeamScoreIsNegative(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Ulid id = Ulid.NewUlid();
        Match match = new(id, homeTeam, awayTeam, matchStartTime);
        _matchRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Ulid>()))
            .Returns(match);

        List<ValidationFailure> failure =
        [
            new ValidationFailure(awayTeam,"Away team score cannot be negative.")
        ];
        _matchValidatorMock.Setup(x => x.ValidateScore(It.IsAny<Match>()))
            .Returns(new ValidationResult(failure));

        var exception = Assert.Throws<ScoreboardException>(() => _scoreboard.UpdateScore(id, homeTeamScore, awayTeamScore));
        Assert.Equal("Match validation failed: Away team score cannot be negative.", exception.Message);
        _matchValidatorMock.Verify(r => r.ValidateScore(It.IsAny<Match>()), Times.Once);
        _matchRepositoryMock.Verify(r => r.GetSingle(It.IsAny<Ulid>()), Times.Once);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void FinishMatch_ShouldRemoveMatchFromScoreboard(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        Ulid id = Ulid.NewUlid();
        Match match = new(id, homeTeam, awayTeam, matchStartTime);
        _matchRepositoryMock.Setup(x => x.GetSingle(It.IsAny<Ulid>()))
            .Returns(match);
        _scoreboard.FinishMatch(id);
        _matchRepositoryMock.Verify(r => r.Remove(match), Times.Once);
    }

    [Fact]
    public void GetMatches_ShouldReturnMatchesOrderedByTotalScoreThenByStartTime()
    {
        _scoreboard.GetMatches();
        _matchRepositoryMock.Verify(r => r.GetAllActive(), Times.Once);
    }
}