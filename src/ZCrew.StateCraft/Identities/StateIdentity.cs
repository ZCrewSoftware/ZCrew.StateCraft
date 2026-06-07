using ZCrew.StateCraft.Identities;

namespace ZCrew.StateCraft;

/// <summary>
///     Factory for creating <see cref="IStateIdentity{TState}"/> instances. Use the generic overloads
///     (<see cref="For{TState, T}"/> and friends) to declare the state's parameter types at compile time, or the
///     <see cref="Type"/>-list overloads when the parameter types are only known at runtime.
/// </summary>
public static class StateIdentity
{
    /// <summary>
    ///     Creates the identity of a parameterless state.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="stateValue">The state value.</param>
    /// <returns>The identity of the state.</returns>
    public static IStateIdentity<TState> For<TState>(TState stateValue)
        where TState : notnull
    {
        return new StateIdentity<TState>(stateValue, []);
    }

    /// <summary>
    ///     Creates the identity of a state with one parameter.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="T">The type of the parameter for this state.</typeparam>
    /// <param name="stateValue">The state value.</param>
    /// <returns>The identity of the state.</returns>
    public static IStateIdentity<TState> For<TState, T>(TState stateValue)
        where TState : notnull
    {
        return new StateIdentity<TState>(stateValue, [typeof(T)]);
    }

    /// <summary>
    ///     Creates the identity of a state with two parameters.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="T1">The type of the first parameter for this state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this state.</typeparam>
    /// <param name="stateValue">The state value.</param>
    /// <returns>The identity of the state.</returns>
    public static IStateIdentity<TState> For<TState, T1, T2>(TState stateValue)
        where TState : notnull
    {
        return new StateIdentity<TState>(stateValue, [typeof(T1), typeof(T2)]);
    }

    /// <summary>
    ///     Creates the identity of a state with three parameters.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="T1">The type of the first parameter for this state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for this state.</typeparam>
    /// <param name="stateValue">The state value.</param>
    /// <returns>The identity of the state.</returns>
    public static IStateIdentity<TState> For<TState, T1, T2, T3>(TState stateValue)
        where TState : notnull
    {
        return new StateIdentity<TState>(stateValue, [typeof(T1), typeof(T2), typeof(T3)]);
    }

    /// <summary>
    ///     Creates the identity of a state with four parameters.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="T1">The type of the first parameter for this state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for this state.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter for this state.</typeparam>
    /// <param name="stateValue">The state value.</param>
    /// <returns>The identity of the state.</returns>
    public static IStateIdentity<TState> For<TState, T1, T2, T3, T4>(TState stateValue)
        where TState : notnull
    {
        return new StateIdentity<TState>(stateValue, [typeof(T1), typeof(T2), typeof(T3), typeof(T4)]);
    }

    /// <summary>
    ///     Creates the identity of a state whose parameter types are supplied at runtime.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="stateValue">The state value.</param>
    /// <param name="stateParameterTypes">The parameter types declared on this state, in declaration order.</param>
    /// <returns>The identity of the state.</returns>
    public static IStateIdentity<TState> For<TState>(TState stateValue, params Type[] stateParameterTypes)
        where TState : notnull
    {
        return new StateIdentity<TState>(stateValue, stateParameterTypes);
    }

    /// <summary>
    ///     Creates the identity of a state whose parameter types are supplied at runtime.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="stateValue">The state value.</param>
    /// <param name="stateParameterTypes">The parameter types declared on this state, in declaration order.</param>
    /// <returns>The identity of the state.</returns>
    public static IStateIdentity<TState> For<TState>(TState stateValue, IEnumerable<Type> stateParameterTypes)
        where TState : notnull
    {
        if (stateParameterTypes is IReadOnlyList<Type> stateParameterTypesList)
        {
            return new StateIdentity<TState>(stateValue, stateParameterTypesList);
        }
        return new StateIdentity<TState>(stateValue, stateParameterTypes.ToArray());
    }
}
