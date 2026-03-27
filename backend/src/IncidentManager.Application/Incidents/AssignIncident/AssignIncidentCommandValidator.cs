using FluentValidation;

namespace IncidentManager.Application.Incidents.AssignIncident;

public sealed class AssignIncidentCommandValidator
    : AbstractValidator<AssignIncidentCommand>
{
    public AssignIncidentCommandValidator()
    {
        RuleFor(x => x.IncidentId).NotEmpty();
        RuleFor(x => x.AssignedTo).NotEmpty();
    }
}