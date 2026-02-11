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
    public void Add(IState<TState, TTransition> state)
    {
        this.states.Add(state);
    }

    /// <summary>
    ///     Looks up a parameterless state by its state value.
    /// </summary>
    /// <param name="stateValue">The state value to match.</param>
    /// <param name="types">The parameter types for the state to match.</param>
    /// <returns>The first matching state.</returns>
    /// <exception cref="InvalidOperationException">No matching state was found.</exception>
    public IState<TState, TTransition> LookupState(TState stateValue, params Type[] types)
    {
        foreach (var state in this.states)
        {
            // Filter out states with other values
            if (!EqualityComparer<TState>.Default.Equals(state.StateValue, stateValue))
            {
                continue;
            }

            if (!state.TypeParameters.IsAssignableFrom(types))
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
        return LookupState(stateValue, typeof(T));
    }

    /// <summary>
    ///     Looks up a state with two typed parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="stateValue">The state value to match.</param>
    /// <returns>The first matching state whose types match.</returns>
    /// <exception cref="InvalidOperationException">No matching state was found.</exception>
    public IState<TState, TTransition> LookupState<T1, T2>(TState stateValue)
    {
        return LookupState(stateValue, typeof(T1), typeof(T2));
    }

    /// <summary>
    ///     Looks up a state with three typed parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="stateValue">The state value to match.</param>
    /// <returns>The first matching state whose types match.</returns>
    /// <exception cref="InvalidOperationException">No matching state was found.</exception>
    public IState<TState, TTransition> LookupState<T1, T2, T3>(TState stateValue)
    {
        return LookupState(stateValue, typeof(T1), typeof(T2), typeof(T3));
    }

    /// <summary>
    ///     Looks up a state with four typed parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="stateValue">The state value to match.</param>
    /// <returns>The first matching state whose types match.</returns>
    /// <exception cref="InvalidOperationException">No matching state was found.</exception>
    public IState<TState, TTransition> LookupState<T1, T2, T3, T4>(TState stateValue)
    {
        return LookupState(stateValue, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
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
