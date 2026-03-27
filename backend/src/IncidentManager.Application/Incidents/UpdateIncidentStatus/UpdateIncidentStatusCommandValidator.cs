using FluentValidation;

namespace IncidentManager.Application.Incidents.UpdateIncidentStatus;

public sealed class UpdateIncidentStatusCommandValidator : AbstractValidator<UpdateIncidentStatusCommand>
{
    public UpdateIncidentStatusCommandValidator()
    {
        RuleFor(x => x.IncidentId).NotEmpty();
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
