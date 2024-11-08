using FootbalScoreboard.Interfaces;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FootbalScoreboardTests")]
namespace FootbalScoreboard.Repositories;
internal class MatchRepository : IMatchRepository
{
    private readonly List<Match> _matches = [];

    public void Add(Match match) => _matches.Add(match);

    public List<Match> GetAllActive() => _matches;

    public Match? GetSingle(string homeTeam, string awayTeam) => _matches.SingleOrDefault(
        m => m.HomeTeam.Equals(homeTeam) && m.AwayTeam.Equals(awayTeam));

    public void Remove(Match match)
    {
        throw new NotImplementedException();
    }
}
