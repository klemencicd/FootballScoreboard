using FluentValidation.Results;
using FootballScoreboard.Models;

namespace FootballScoreboard.Interfaces;

public interface IMatchValidator
{
    ValidationResult ValidateStart(Match match, List<Match> existingMatches);
    ValidationResult ValidateScore(Match match);
}