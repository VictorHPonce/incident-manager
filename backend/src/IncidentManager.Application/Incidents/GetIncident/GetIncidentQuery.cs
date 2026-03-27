using IncidentManager.Application.Common.Interfaces;
using IncidentManager.Application.Incidents.DTOs;

namespace IncidentManager.Application.Incidents.GetIncident;

public record GetIncidentQuery(Guid Id) : IQuery<IncidentResponse>;
