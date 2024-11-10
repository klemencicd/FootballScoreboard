using FluentValidation;
using FluentValidation.Results;
using FootballScoreboard.Interfaces;

namespace FootballScoreboard.Models.Validations;
internal class MatchValidator : AbstractValidator<Match>, IMatchValidator
{
    public MatchValidator(List<Match>? existingMatches = null)
    {
        RuleFor(x => x)
            .Must(match => match.HomeTeam != match.AwayTeam)
            .WithMessage("Home team and away team must be different.");

        RuleFor(x => x.StartTime)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Match start time cannot be in the future.");

        RuleFor(x => x.HomeTeam)
            .NotEmpty().WithMessage("Home team cannot be empty.");

        RuleFor(x => x.AwayTeam)
            .NotEmpty().WithMessage("Away team cannot be empty.");

        if (existingMatches != null)
        {
            RuleFor(x => x.HomeTeam)
                .Must((match, homeTeam) => !existingMatches.Any(m => m.HomeTeam == homeTeam || m.AwayTeam == homeTeam))
                .WithMessage(match => $"Team {match.HomeTeam} is already in a match.");

            RuleFor(x => x.AwayTeam)
                .Must((match, awayTeam) => !existingMatches.Any(m => m.HomeTeam == awayTeam || m.AwayTeam == awayTeam))
                .WithMessage(match => $"Team {match.AwayTeam} is already in a match.");
        }

        RuleFor(x => x.HomeTeamScore)
            .GreaterThanOrEqualTo(0).WithMessage("Home team score cannot be negative.");

        RuleFor(x => x.AwayTeamScore)
            .GreaterThanOrEqualTo(0).WithMessage("Away team score cannot be negative.");
    }

    public ValidationResult ValidateStart(Match match, List<Match> existingMatches)
    {
        MatchValidator validator = new(existingMatches);
        return validator.Validate(match);
    }

    public ValidationResult ValidateScore(Match match)
    {
        return Validate(match);
    }
}
