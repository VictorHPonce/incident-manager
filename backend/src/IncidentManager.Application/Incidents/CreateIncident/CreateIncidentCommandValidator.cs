using FluentValidation;

namespace IncidentManager.Application.Incidents.CreateIncident;

public sealed class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.TeamId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
