using FootbalScoreboard.Interfaces;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FootbalScoreboardTests")]
namespace FootbalScoreboard.Repositories;
internal class MatchRepository : IMatchRepository
{
    private readonly List<Match> _matches = [];

    public void Add(Match match)
    {
        throw new NotImplementedException();
    }

    public List<Match> GetAllActive()
    {
        throw new NotImplementedException();
    }

    public Match? GetSingle(string homeTeam, string awayTeam)
    {
        throw new NotImplementedException();
    }

    public void Remove(Match match)
    {
        throw new NotImplementedException();
    }
}
