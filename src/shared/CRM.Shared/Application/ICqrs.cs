using MediatR;

namespace CRM.Shared.Application;

// CQRS Interfaces — Commands change state; Queries read state.

/// <summary>Command that returns a typed result.</summary>
public interface ICommand<out TResult> : IRequest<TResult> { }

/// <summary>Command that returns nothing.</summary>
public interface ICommand : IRequest { }

/// <summary>Handler for typed commands.</summary>
public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult> { }

/// <summary>Handler for void commands.</summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand { }

/// <summary>Query that returns a typed result.</summary>
public interface IQuery<out TResult> : IRequest<TResult> { }

/// <summary>Handler for queries.</summary>
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult> { }
