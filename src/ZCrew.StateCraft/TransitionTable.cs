using System.Collections;
using System.Diagnostics;
using ZCrew.StateCraft.Transitions.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Organizes transitions by type parameter count and provides type-safe lookups.
///     Returns the first transition matching the value, type constraints, and conditions.
/// </summary>
/// <typeparam name="TState">The state enumeration type.</typeparam>
/// <typeparam name="TTransition">The transition enumeration type.</typeparam>
[DebuggerDisplay("Count = {Count}")]
internal sealed class TransitionTable<TState, TTransition> : IEnumerable<ITransition<TState, TTransition>>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IParameterlessTransition<TState, TTransition>> parameterlessTransitions = [];
    private readonly List<ITransition<TState, TTransition>> previousOnlyTransitions = [];
    private readonly List<ITransition<TState, TTransition>> nextOnlyTransitions = [];
    private readonly List<ITransition<TState, TTransition>> bothTransitions = [];

    /// <summary>
    ///     Gets the total number of transitions in this table.
    /// </summary>
    private int Count =>
        this.parameterlessTransitions.Count
        + this.previousOnlyTransitions.Count
        + this.nextOnlyTransitions.Count
        + this.bothTransitions.Count;

    public TransitionTable() { }

    public TransitionTable(IEnumerable<ITransition<TState, TTransition>> transitions)
    {
        foreach (var transition in transitions)
        {
            Add(transition);
        }
    }

    /// <summary>
    ///     Adds the <paramref name="transition"/> to the table.
    /// </summary>
    /// <param name="transition">The transition to add to the table.</param>
    /// <exception cref="InvalidOperationException">If the parameter count of the transition is unexpected.</exception>
    public void Add(ITransition<TState, TTransition> transition)
    {
        var previousParameterCount = transition.PreviousStateTypeParameters.Count;
        var transitionParameterCount = transition.TransitionTypeParameters.Count;
        switch (previousParameterCount, transitionParameterCount)
        {
            case (0, 0):
                // Since there are no type parameters the type can be safely cast here once
                this.parameterlessTransitions.Add((IParameterlessTransition<TState, TTransition>)transition);
                break;
            case (1, 0):
                this.previousOnlyTransitions.Add(transition);
                break;
            case (0, 1):
                this.nextOnlyTransitions.Add(transition);
                break;
            case (1, 1):
                this.bothTransitions.Add(transition);
                break;
            default:
                throw new InvalidOperationException(
                    $"Unable to handle transition with parameter counts: {previousParameterCount}, {transitionParameterCount}"
                );
        }
    }

    /// <summary>
    ///     Looks up a parameterless transition by its transition value.
    /// </summary>
    /// <param name="transition">The transition value to match.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<IParameterlessTransition<TState, TTransition>?> LookupParameterlessTransition(
        TTransition transition,
        CancellationToken token
    )
    {
        foreach (var parameterlessTransition in this.parameterlessTransitions)
        {
            // Filter out transitions with other values
            if (!EqualityComparer<TTransition>.Default.Equals(parameterlessTransition.TransitionValue, transition))
            {
                continue;
            }

            if (!await parameterlessTransition.EvaluateConditions(token))
            {
                continue;
            }

            return parameterlessTransition;
        }

        return null;
    }

    /// <summary>
    ///     Looks up a transition with typed previous state parameter.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous state's parameter.</typeparam>
    /// <param name="transition">The transition value to match.</param>
    /// <param name="previous">The previous state parameter for type matching and condition evaluation.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<IParameterlessTransition<
        TState,
        TTransition,
        TPrevious
    >?> LookupParameterlessTransition<TPrevious>(TTransition transition, TPrevious previous, CancellationToken token)
    {
        foreach (var previousOnlyTransition in this.previousOnlyTransitions)
        {
            // Filter out transitions with other values
            if (!EqualityComparer<TTransition>.Default.Equals(previousOnlyTransition.TransitionValue, transition))
            {
                continue;
            }

            // Filter out transitions with a different type
            if (previousOnlyTransition is not IParameterlessTransition<TState, TTransition, TPrevious> typedTransition)
            {
                continue;
            }

            if (!await typedTransition.EvaluateConditions(previous, token))
            {
                continue;
            }

            return typedTransition;
        }

        return null;
    }

    /// <summary>
    ///     Looks up a transition with typed next state parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the next state's parameter.</typeparam>
    /// <param name="transition">The transition value to match.</param>
    /// <param name="next">The next state parameter for type matching and condition evaluation.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<IParameterizedTransition<TState, TTransition, TNext>?> LookupParameterizedTransition<TNext>(
        TTransition transition,
        TNext next,
        CancellationToken token
    )
    {
        foreach (var nextOnlyTransition in this.nextOnlyTransitions)
        {
            // Filter out transitions with other values
            if (!EqualityComparer<TTransition>.Default.Equals(nextOnlyTransition.TransitionValue, transition))
            {
                continue;
            }

            // Filter out transitions with a different type
            if (nextOnlyTransition is not IParameterizedTransition<TState, TTransition, TNext> typedTransition)
            {
                continue;
            }

            if (!await typedTransition.EvaluateConditions(next, token))
            {
                continue;
            }

            return typedTransition;
        }

        return null;
    }

    /// <summary>
    ///     Looks up a transition with both typed previous and next state parameter.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous state's parameter.</typeparam>
    /// <typeparam name="TNext">The type of the next state's parameter.</typeparam>
    /// <param name="transition">The transition value to match.</param>
    /// <param name="previous">The previous state parameter for type matching and condition evaluation.</param>
    /// <param name="next">The next state parameter for type matching and condition evaluation.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<IParameterizedTransition<TState, TTransition, TPrevious, TNext>?> LookupParameterizedTransition<
        TPrevious,
        TNext
    >(TTransition transition, TPrevious previous, TNext next, CancellationToken token)
    {
        foreach (var bothTransition in this.bothTransitions)
        {
            // Filter out transitions with other values
            if (!EqualityComparer<TTransition>.Default.Equals(bothTransition.TransitionValue, transition))
            {
                continue;
            }

            // Filter out transitions with a different type
            if (bothTransition is not IParameterizedTransition<TState, TTransition, TPrevious, TNext> typedTransition)
            {
                continue;
            }

            if (!await typedTransition.EvaluateConditions(previous, next, token))
            {
                continue;
            }

            return typedTransition;
        }

        return null;
    }

    /// <inheritdoc/>
    public IEnumerator<ITransition<TState, TTransition>> GetEnumerator()
    {
        return this
            .parameterlessTransitions.OfType<ITransition<TState, TTransition>>()
            .Concat(this.previousOnlyTransitions)
            .Concat(this.nextOnlyTransitions)
            .Concat(this.bothTransitions)
            .GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
