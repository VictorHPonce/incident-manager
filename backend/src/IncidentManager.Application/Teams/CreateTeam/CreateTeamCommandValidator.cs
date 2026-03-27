using FluentValidation;

namespace IncidentManager.Application.Teams.CreateTeam;

public sealed class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del equipo es obligatorio.")
            .MaximumLength(100);
    }
}