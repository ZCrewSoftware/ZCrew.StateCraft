using System.Collections;
using System.Diagnostics;
using ZCrew.StateCraft.States.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Organizes states by type parameter count and provides type-safe lookups.
///     Returns the first state matching the value and type constraints.
/// </summary>
/// <typeparam name="TState">The state enumeration type.</typeparam>
/// <typeparam name="TTransition">The transition enumeration type.</typeparam>
[DebuggerDisplay("Count = {Count}")]
internal sealed class StateTable<TState, TTransition> : IEnumerable<IState<TState, TTransition>>
    where TState : notnull
    where TTransition : notnull
{
    private readonly List<IParameterlessState<TState, TTransition>> parameterlessStates = [];
    private readonly List<IState<TState, TTransition>> parameterizedStates = [];

    /// <summary>
    ///     Gets the total number of states in this table.
    /// </summary>
    private int Count => this.parameterlessStates.Count + this.parameterizedStates.Count;

    public StateTable() { }

    public StateTable(IEnumerable<IState<TState, TTransition>> states)
    {
        foreach (var state in states)
        {
            Add(state);
        }
    }

    /// <summary>
    ///     Adds the <paramref name="state"/> to the table.
    /// </summary>
    /// <param name="state">The state to add to the table.</param>
    /// <exception cref="InvalidOperationException">If the parameter count of the state is unexpected.</exception>
    public void Add(IState<TState, TTransition> state)
    {
        var parameterCount = state.TypeParameters.Count;
        switch (parameterCount)
        {
            case 0:
                // Since there are no type parameters the type can be safely cast here once
                this.parameterlessStates.Add((IParameterlessState<TState, TTransition>)state);
                break;
            case 1:
                this.parameterizedStates.Add(state);
                break;
            default:
                throw new InvalidOperationException($"Unable to handle state with parameter count: {parameterCount}");
        }
    }

    /// <summary>
    ///     Looks up a parameterless state by its state value.
    /// </summary>
    /// <param name="state">The state value to match.</param>
    /// <returns>The first matching state.</returns>
    /// <exception cref="InvalidOperationException">No matching state was found.</exception>
    public IParameterlessState<TState, TTransition> LookupState(TState state)
    {
        foreach (var parameterlessState in this.parameterlessStates)
        {
            if (EqualityComparer<TState>.Default.Equals(parameterlessState.StateValue, state))
            {
                return parameterlessState;
            }
        }

        throw new InvalidOperationException($"No parameterless state could be found for: State={state}");
    }

    /// <summary>
    ///     Looks up a state with a typed parameter.
    /// </summary>
    /// <typeparam name="T">The type of the state's parameter.</typeparam>
    /// <param name="state">The state value to match.</param>
    /// <returns>The first matching state whose type matches.</returns>
    /// <exception cref="InvalidOperationException">No matching state was found.</exception>
    public IParameterizedState<TState, TTransition, T> LookupState<T>(TState state)
    {
        foreach (var parameterizedState in this.parameterizedStates)
        {
            // Filter out states with other values
            if (!EqualityComparer<TState>.Default.Equals(parameterizedState.StateValue, state))
            {
                continue;
            }

            // Filter out states with a different type
            if (parameterizedState is IParameterizedState<TState, TTransition, T> typedState)
            {
                return typedState;
            }
        }

        throw new InvalidOperationException($"No state could be found for: State={state}, ParameterType={typeof(T)}");
    }

    /// <inheritdoc/>
    public IEnumerator<IState<TState, TTransition>> GetEnumerator()
    {
        return this
            .parameterlessStates.OfType<IState<TState, TTransition>>()
            .Concat(this.parameterizedStates)
            .GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
