using FootballScoreboard.Models;
using FootballScoreboard.Models.Validations;

namespace FootballScoreboardTests;
public class MatchValidatorTests
{
    private readonly MatchValidator _validator = new();

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void ValidateStart_WithNoExistingMatch_ReturnsValidResult(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        var existingMatches = new List<Match>
        {
            new(Ulid.NewUlid(), "Spain", "Brazil", matchStartTime)
        };

        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);

        var result = _validator.ValidateStart(match, existingMatches);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("Mexico", "Mexico")]
    public void ValidateStart_WithSameHomeAndAwayTeam_ReturnsError(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);

        var result = _validator.ValidateStart(match, []);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Home team and away team must be different.", result.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void ValidateStart_WithMatchTimeInTheFuture_ReturnsError(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow.AddHours(1);
        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);

        var result = _validator.ValidateStart(match, []);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Match start time cannot be in the future.", result.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void ValidateStart_WithExistingMatch_ReturnsErrorForHomeTeamInMatch(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        var existingMatches = new List<Match>
        {
            new(Ulid.NewUlid(), homeTeam, "Brazil", matchStartTime)
        };

        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);

        var result = _validator.ValidateStart(match, existingMatches);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Team Mexico is already in a match.", result.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void ValidateStart_WithExistingMatch_ReturnsErrorForAwayTeamInMatch(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        var existingMatches = new List<Match>
        {
            new(Ulid.NewUlid(), "Brazil", awayTeam, matchStartTime)
        };

        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);

        var result = _validator.ValidateStart(match, existingMatches);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Team Canada is already in a match.", result.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData("", "Canada")]
    public void ValidateStart_WithEmptyHomeTeam_ReturnsError(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);

        var result = _validator.ValidateStart(match, []);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Home team cannot be empty.", result.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData("Mexico", "")]
    public void ValidateStart_WithEmptyAwayTeam_ReturnsError(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);

        var result = _validator.ValidateStart(match, []);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Away team cannot be empty.", result.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData("Mexico", "Canada", 1, 2)]
    public void ValidateScore_WithValidScores_ReturnsNoError(
        string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);
        match.UpdateScore(homeTeamScore, awayTeamScore);
        var result = _validator.ValidateStart(match, []);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("Mexico", "Canada", -1, 2)]
    public void ValidateScore_WithNegativeHomeTeamScore_ReturnsError(
        string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);
        match.UpdateScore(homeTeamScore, awayTeamScore);
        var result = _validator.ValidateStart(match, []);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Home team score cannot be negative.", result.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData("Mexico", "Canada", 1, -2)]
    public void ValidateScore_WithNegativeAwayTeamScore_ReturnsError(
        string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);
        match.UpdateScore(homeTeamScore, awayTeamScore);
        var result = _validator.ValidateStart(match, []);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Away team score cannot be negative.", result.Errors[0].ErrorMessage);
    }
}
