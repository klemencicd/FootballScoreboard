using FootballScoreboard;
using FootballScoreboard.Interfaces;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FootballScoreboardTests")]
namespace FootballScoreboard.Repositories;
internal class MatchRepository : IMatchRepository
{
    private readonly List<Match> _matches = [];

    public void Add(Match match) => _matches.Add(match);

    public List<Match> GetAllActive() => [.. _matches
            .OrderByDescending(x => x.HomeTeamScore + x.AwayTeamScore)
            .ThenByDescending(x => x.StartTime)];

    public Match? GetSingle(string homeTeam, string awayTeam) => _matches.SingleOrDefault(
        m => m.HomeTeam.Equals(homeTeam) && m.AwayTeam.Equals(awayTeam));

    public void Remove(Match match) => _matches.Remove(match);
}
