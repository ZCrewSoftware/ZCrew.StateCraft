namespace ZCrew.StateCraft;

/// <summary>
///     The initial state configuration which provides an opportunity to specify parameters. This can only be done at
///     the start since other transitions will depend on the existence of parameters.
/// </summary>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
/// <typeparam name="TTransition">
///     The transition type. This should be an <see langword="enum"/> type or it should be an equatable type so the
///     state machine behaves as expected.
/// </typeparam>
public interface IInitialStateConfiguration<TState, TTransition> : IParameterlessStateConfiguration<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Configures the state to have no parameters. This is optional and is implied if no parameter configuration
    ///     is explicitly made, such as calling <see cref="WithParameter{T}"/>.
    /// </summary>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterlessStateConfiguration<TState, TTransition> WithNoParameters();

    /// <summary>
    ///     Configures the state to have one parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter for this state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T> WithParameter<T>();

    /// <summary>
    ///     Configures the state to have two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for this state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2> WithParameters<T1, T2>();

    /// <summary>
    ///     Configures the state to have three parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for this state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for this state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3> WithParameters<T1, T2, T3>();

    /// <summary>
    ///     Configures the state to have four parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter for this state.</typeparam>
    /// <typeparam name="T2">The type of the second parameter for this state.</typeparam>
    /// <typeparam name="T3">The type of the third parameter for this state.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter for this state.</typeparam>
    /// <returns>A reference to the configuration after the configuration was updated.</returns>
    IParameterizedStateConfiguration<TState, TTransition, T1, T2, T3, T4> WithParameters<T1, T2, T3, T4>();
}
