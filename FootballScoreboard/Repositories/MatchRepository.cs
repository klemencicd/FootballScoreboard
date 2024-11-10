using FootballScoreboard.Interfaces;
using FootballScoreboard.Models;
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

    public Match? GetSingle(Ulid id) => 
        _matches.SingleOrDefault(m => m.Id.Equals(id));

    public void Remove(Match match) => _matches.Remove(match);
}
