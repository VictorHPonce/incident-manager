using MediatR;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Common.Interfaces;

/// <summary>Query que devuelve Result&lt;T&gt;.</summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
