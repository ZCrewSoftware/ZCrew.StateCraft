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
    ///     Looks up a parameterless transition by its transition value.
    /// </summary>
    /// <param name="transitionValue">The transition value to match.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<ITransition<TState, TTransition>?> LookupParameterlessTransition(
        TTransition transitionValue,
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

            // TODO MWZ: this pattern is messy and could be replaced once the state machine is refactored
            if (transition.Previous.State.TypeParameters.Count != 0)
            {
                continue;
            }

            if (transition.TransitionTypeParameters.Count != 0)
            {
                continue;
            }

            // TODO MWZ: just pass in the correct state machine parameters this after the state machine is refactored
            var emptyStateMachineParameters = new StateMachineParameters();
            if (!await transition.EvaluateConditions(emptyStateMachineParameters, token))
            {
                continue;
            }

            return transition;
        }

        return null;
    }

    /// <summary>
    ///     Looks up a transition with typed previous state parameter.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous state's parameter.</typeparam>
    /// <param name="transitionValue">The transition value to match.</param>
    /// <param name="previous">The previous state parameter for type matching and condition evaluation.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<ITransition<TState, TTransition>?> LookupParameterlessTransition<TPrevious>(
        TTransition transitionValue,
        TPrevious previous,
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

            // TODO MWZ: this pattern is messy and could be replaced once the state machine is refactored
            if (transition.Previous.State.TypeParameters.Count != 1)
            {
                continue;
            }

            if (!transition.Previous.State.TypeParameters[0].IsAssignableFrom(typeof(TPrevious)))
            {
                continue;
            }

            if (transition.TransitionTypeParameters.Count != 0)
            {
                continue;
            }

            // TODO MWZ: just pass in the correct state machine parameters this after the state machine is refactored
            var stateMachineParameters = new StateMachineParameters();
            stateMachineParameters.SetNextParameter(previous);
            stateMachineParameters.CommitTransition();
            stateMachineParameters.BeginTransition();
            if (!await transition.EvaluateConditions(stateMachineParameters, token))
            {
                continue;
            }

            return transition;
        }

        return null;
    }

    /// <summary>
    ///     Looks up a transition with typed next state parameter.
    /// </summary>
    /// <typeparam name="TNext">The type of the next state's parameter.</typeparam>
    /// <param name="transitionValue">The transition value to match.</param>
    /// <param name="next">The next state parameter for type matching and condition evaluation.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<ITransition<TState, TTransition>?> LookupParameterizedTransition<TNext>(
        TTransition transitionValue,
        TNext next,
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

            // TODO MWZ: this pattern is messy and could be replaced once the state machine is refactored
            if (transition.Previous.State.TypeParameters.Count != 0)
            {
                continue;
            }

            if (transition.TransitionTypeParameters.Count != 1)
            {
                continue;
            }

            if (!transition.TransitionTypeParameters[0].IsAssignableFrom(typeof(TNext)))
            {
                continue;
            }

            // TODO MWZ: just pass in the correct state machine parameters this after the state machine is refactored
            var stateMachineParameters = new StateMachineParameters();
            stateMachineParameters.SetNextParameter(next);
            if (!await transition.EvaluateConditions(stateMachineParameters, token))
            {
                continue;
            }

            return transition;
        }

        return null;
    }

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
            if (transition.TransitionTypeParameters.Count != 0)
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

    public async Task<ITransition<TState, TTransition>?> LookupTransition<T>(
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
            if (transition.TransitionTypeParameters.IsAssignableFrom([typeof(T)]))
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

    /// <summary>
    ///     Looks up a transition with both typed previous and next state parameter.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous state's parameter.</typeparam>
    /// <typeparam name="TNext">The type of the next state's parameter.</typeparam>
    /// <param name="transitionValue">The transition value to match.</param>
    /// <param name="previous">The previous state parameter for type matching and condition evaluation.</param>
    /// <param name="next">The next state parameter for type matching and condition evaluation.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The first matching transition whose conditions evaluate to <see langword="true"/>.
    ///     Otherwise, <see langword="null"/> if no transition can be found.
    /// </returns>
    public async Task<ITransition<TState, TTransition>?> LookupParameterizedTransition<TPrevious, TNext>(
        TTransition transitionValue,
        TPrevious previous,
        TNext next,
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

            // TODO MWZ: this pattern is messy and could be replaced once the state machine is refactored
            if (transition.Previous.State.TypeParameters.Count != 1)
            {
                continue;
            }

            if (!transition.Previous.State.TypeParameters[0].IsAssignableFrom(typeof(TPrevious)))
            {
                continue;
            }

            if (transition.TransitionTypeParameters.Count != 1)
            {
                continue;
            }

            if (!transition.TransitionTypeParameters[0].IsAssignableFrom(typeof(TNext)))
            {
                continue;
            }

            // TODO MWZ: just pass in the correct state machine parameters this after the state machine is refactored
            var stateMachineParameters = new StateMachineParameters();
            stateMachineParameters.SetNextParameter(previous);
            stateMachineParameters.CommitTransition();
            stateMachineParameters.BeginTransition();
            stateMachineParameters.SetNextParameter(next);
            if (!await transition.EvaluateConditions(stateMachineParameters, token))
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
