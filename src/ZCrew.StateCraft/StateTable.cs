using System.Collections;
using System.Diagnostics;

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
    private readonly List<IState<TState, TTransition>> states;

    /// <summary>
    ///     Gets the total number of states in this table.
    /// </summary>
    private int Count => this.states.Count;

    public StateTable()
    {
        this.states = [];
    }

    public StateTable(IEnumerable<IState<TState, TTransition>> states)
    {
        this.states = states.ToList();
    }

    /// <summary>
    ///     Adds the <paramref name="state"/> to the table.
    /// </summary>
    /// <param name="state">The state to add to the table.</param>
    /// <exception cref="InvalidOperationException">If the parameter count of the state is unexpected.</exception>
    public void Add(IState<TState, TTransition> state)
    {
        this.states.Add(state);
    }

    /// <summary>
    ///     Looks up a parameterless state by its state value.
    /// </summary>
    /// <param name="stateValue">The state value to match.</param>
    /// <returns>The first matching state.</returns>
    /// <exception cref="InvalidOperationException">No matching state was found.</exception>
    public IState<TState, TTransition> LookupState(TState stateValue)
    {
        foreach (var state in this.states)
        {
            // Filter out states with other values
            if (!EqualityComparer<TState>.Default.Equals(state.StateValue, stateValue))
            {
                continue;
            }

            if (state.TypeParameters.Count != 0)
            {
                continue;
            }

            return state;
        }

        throw new InvalidOperationException($"No parameterless state could be found for: State={stateValue}");
    }

    /// <summary>
    ///     Looks up a state with a typed parameter.
    /// </summary>
    /// <typeparam name="T">The type of the state's parameter.</typeparam>
    /// <param name="stateValue">The state value to match.</param>
    /// <returns>The first matching state whose type matches.</returns>
    /// <exception cref="InvalidOperationException">No matching state was found.</exception>
    public IState<TState, TTransition> LookupState<T>(TState stateValue)
    {
        foreach (var state in this.states)
        {
            // Filter out states with other values
            if (!EqualityComparer<TState>.Default.Equals(state.StateValue, stateValue))
            {
                continue;
            }

            if (state.TypeParameters.Count != 1)
            {
                continue;
            }

            if (!state.TypeParameters.Single().IsAssignableFrom(typeof(T)))
            {
                continue;
            }

            return state;
        }

        throw new InvalidOperationException(
            $"No state could be found for: State={stateValue}, ParameterType={typeof(T)}"
        );
    }

    /// <inheritdoc/>
    public IEnumerator<IState<TState, TTransition>> GetEnumerator()
    {
        return this.states.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
