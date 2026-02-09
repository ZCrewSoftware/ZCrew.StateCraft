using System.Collections;
using System.Diagnostics;
using ZCrew.StateCraft.Parameters;
using ZCrew.StateCraft.Parameters.Contracts;
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
    private readonly List<ITransition<TState, TTransition>> transitions;

    /// <summary>
    ///     Gets the total number of transitions in this table.
    /// </summary>
    private int Count => this.transitions.Count;

    public TransitionTable()
    {
        this.transitions = [];
    }

    public TransitionTable(IEnumerable<ITransition<TState, TTransition>> transitions)
    {
        this.transitions = transitions.ToList();
    }

    /// <summary>
    ///     Adds the <paramref name="transition"/> to the table.
    /// </summary>
    /// <param name="transition">The transition to add to the table.</param>
    /// <exception cref="InvalidOperationException">If the parameter count of the transition is unexpected.</exception>
    public void Add(ITransition<TState, TTransition> transition)
    {
        this.transitions.Add(transition);
    }

    /// <summary>
    ///     Looks up a transition by its transition value, parameters and conditions.
    /// </summary>
    /// <param name="transitionValue">The transition value to match.</param>
    /// <param name="parameters">The current parameters and transition parameters.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<ITransition<TState, TTransition>?> LookupTransition(
        TTransition transitionValue,
        IStateMachineParameters parameters,
        CancellationToken token
    )
    {
        foreach (var transition in this.transitions)
        {
            // Filter out transitions with other values
            if (!EqualityComparer<TTransition>.Default.Equals(transition.TransitionValue, transitionValue))
            {
                continue;
            }

            // Filter out transitions with parameters
            if (!transition.TransitionTypeParameters.IsAssignableFrom(parameters.NextParameterTypes))
            {
                continue;
            }

            // Filter out transitions that failed conditions
            if (!await transition.EvaluateConditions(parameters, token))
            {
                continue;
            }

            return transition;
        }

        return null;
    }

    /// <inheritdoc/>
    public IEnumerator<ITransition<TState, TTransition>> GetEnumerator()
    {
        return this.transitions.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
