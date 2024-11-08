using FootballScoreboard.Interfaces;
using FootballScoreboard.Exceptions;
using FootballScoreboard.Models.Validations;
using FluentValidation.Results;

namespace FootballScoreboard.Models;

public class Scoreboard(IMatchRepository _matchRepository) : IScoreboard
{
    public void StartMatch(string homeTeam, string awayTeam, DateTime matchStartTime)
    {
        Match match = new(homeTeam, awayTeam, matchStartTime);

        MatchValidator validator = new(_matchRepository.GetAllActive());
        ValidationResult result = validator.Validate(match);

        if (!result.IsValid)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(error => error.ErrorMessage));

            throw new ScoreboardException($"Match validation failed: {errorMessage}");
        }

        _matchRepository.Add(match);
    }

    public void UpdateScore(string homeTeam, string awayTeam, int homeTeamScore, int awayTeamScore)
    {
        Match? match = _matchRepository.GetSingle(homeTeam, awayTeam);
        if (match == null) return;

        match.UpdateScore(homeTeamScore, awayTeamScore);

        MatchValidator validator = new();
        ValidationResult result = validator.Validate(match);

        if (!result.IsValid)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(error => error.ErrorMessage));

            throw new ScoreboardException($"Match validation failed: {errorMessage}");
        }
    }

    public void FinishMatch(string homeTeam, string awayTeam)
    {
        Match? match = _matchRepository.GetSingle(homeTeam, awayTeam);
        if (match != null)
        {
            _matchRepository.Remove(match);
        }
    }

    public List<Match> GetMatches()
    {
        return _matchRepository.GetAllActive();
    }
}
