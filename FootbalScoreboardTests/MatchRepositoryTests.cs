﻿using FootbalScoreboard;
using FootbalScoreboard.Repositories;

namespace FootbalScoreboardTests;
public class MatchRepositoryTests
{
    private readonly MatchRepository _matchRepository = new();

    [Theory]
    [InlineData("Mexico", "Canada")]
    public void Add_ShouldStartNewMatch_WithInitialScoreZero(string homeTeam, string awayTeam)
    {
        var matchStartTime = DateTime.UtcNow;
        Match match = new(homeTeam, awayTeam, matchStartTime); 
        _matchRepository.Add(match);
        List<Match> matches = _matchRepository.GetAllActive();

        Assert.Single(matches);
        Assert.Equal(homeTeam, matches[0].HomeTeam);
        Assert.Equal(0, matches[0].HomeTeamScore);
        Assert.Equal(awayTeam, matches[0].AwayTeam);
        Assert.Equal(0, matches[0].AwayTeamScore);
        Assert.Equal(matchStartTime, matches[0].StartTime);
    }
}
