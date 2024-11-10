using FootballScoreboard.Models;
using FootballScoreboard.Repositories;

namespace FootballScoreboardTests;
public class MatchRepositoryTests
{
    private readonly MatchRepository _matchRepository = new();

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void Add_ShouldStartNewMatch_WithInitialScoreZero(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        Ulid id = Ulid.NewUlid();
        Match match = new(id, homeTeam, awayTeam, matchStartTime);
        _matchRepository.Add(match);
        List<Match> matches = _matchRepository.GetAllActive();

        Assert.Single(matches);
        Assert.Equal(id, matches[0].Id);
        Assert.Equal(homeTeam, matches[0].HomeTeam);
        Assert.Equal(0, matches[0].HomeTeamScore);
        Assert.Equal(awayTeam, matches[0].AwayTeam);
        Assert.Equal(0, matches[0].AwayTeamScore);
        Assert.Equal(matchStartTime, matches[0].StartTime);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void GetSingle_ShouldReturnCorrectMatch(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        Ulid id = Ulid.NewUlid();
        Match match = new(id, homeTeam, awayTeam, matchStartTime);
        _matchRepository.Add(match);
        Match? addeMatch = _matchRepository.GetSingle(id);

        Assert.NotNull(addeMatch);
        Assert.Equal(id, addeMatch.Id);
        Assert.Equal(homeTeam, addeMatch.HomeTeam);
        Assert.Equal(0, addeMatch.HomeTeamScore);
        Assert.Equal(awayTeam, addeMatch.AwayTeam);
        Assert.Equal(0, addeMatch.AwayTeamScore);
        Assert.Equal(matchStartTime, addeMatch.StartTime);
        Assert.Equal(match, addeMatch);
    }

    [Fact]
    public void GetSingle_ShouldReturnNull_WhenMatchDoesNotExist()
    {
        Match? addeMatch = _matchRepository.GetSingle(Ulid.NewUlid());
        Assert.Null(addeMatch);
    }

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void Remove_ShouldDeleteMatch(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(Ulid.NewUlid(), homeTeam, awayTeam, matchStartTime);
        _matchRepository.Add(match);
        _matchRepository.Remove(match);
        List<Match> matches = _matchRepository.GetAllActive();

        Assert.Empty(matches);
    }

    [Fact]
    public void GetAllActive_ShouldReturnMatchesOrderedByTotalScoreThenByStartTime()
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(Ulid.NewUlid(), "Mexico", "Canada", matchStartTime);
        match.UpdateScore(0, 5);
        _matchRepository.Add(match);
        match = new(Ulid.NewUlid(), "Spain", "Brazil", matchStartTime.AddMinutes(1));
        match.UpdateScore(10, 2);
        _matchRepository.Add(match);
        match = new(Ulid.NewUlid(), "Germany", "France", matchStartTime.AddMinutes(2));
        match.UpdateScore(2, 2);
        _matchRepository.Add(match);
        match = new(Ulid.NewUlid(), "Uruguay", "Italy", matchStartTime.AddMinutes(3));
        match.UpdateScore(6, 6);
        _matchRepository.Add(match);
        match = new(Ulid.NewUlid(), "Argentina", "Australia", matchStartTime.AddMinutes(4));
        match.UpdateScore(3, 1);
        _matchRepository.Add(match);

        List<Match> matches = _matchRepository.GetAllActive();

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

    [Fact]
    public void GetAllActive_ShouldReturnEmptyList_IfNoMatches()
    {
        List<Match> matches = _matchRepository.GetAllActive();
        Assert.Empty(matches);
    }
}
