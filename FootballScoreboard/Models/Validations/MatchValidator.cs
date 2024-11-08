using FluentValidation;

namespace FootballScoreboard.Models.Validations;
internal class MatchValidator : AbstractValidator<Match>
{
    public MatchValidator(List<Match>? existingMatches = null)
    {
        if (existingMatches != null)
        {
            RuleFor(x => x.HomeTeam)
                .NotEmpty().WithMessage("Home team cannot be empty.")
                .Must((match, homeTeam) => !existingMatches.Any(m => m.HomeTeam == homeTeam || m.AwayTeam == homeTeam))
                .WithMessage(match => $"Team {match.HomeTeam} is already in a match.");

            RuleFor(x => x.AwayTeam)
                .NotEmpty().WithMessage("Away team cannot be empty.")
                .Must((match, awayTeam) => !existingMatches.Any(m => m.HomeTeam == awayTeam || m.AwayTeam == awayTeam))
                .WithMessage(match => $"Team {match.AwayTeam} is already in a match.");
        }

        RuleFor(x => x.HomeTeamScore)
            .GreaterThanOrEqualTo(0).WithMessage("Home team score cannot be negative.");

        RuleFor(x => x.AwayTeamScore)
            .GreaterThanOrEqualTo(0).WithMessage("Away team score cannot be negative.");
    }
}
