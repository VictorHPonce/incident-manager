using MediatR;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Common.Interfaces;

/// <summary>Command que devuelve Result&lt;T&gt; — para operaciones con valor de retorno.</summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }

/// <summary>Command que devuelve Result — para operaciones sin valor de retorno.</summary>
public interface ICommand : IRequest<Result> { }
