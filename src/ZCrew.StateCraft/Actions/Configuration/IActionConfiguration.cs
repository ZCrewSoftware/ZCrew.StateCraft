using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents common functionality to describe a parameterless state action configuration regardless of the
///     semantics of the action.
/// </summary>
public interface IActionConfiguration
{
    /// <summary>
    ///     Builds the action based on the configuration.
    /// </summary>
    /// <returns>The action model.</returns>
    internal IAction Build();
}

/// <summary>
///     Represents common functionality to describe a parameterized state action configuration regardless of the
///     semantics of the action.
/// </summary>
/// <typeparam name="T">The type of the parameter passed to the action.</typeparam>
public interface IActionConfiguration<in T>
{
    /// <summary>
    ///     Builds the action based on the configuration.
    /// </summary>
    /// <returns>The action model.</returns>
    internal IAction<T> Build();
}

/// <summary>
///     Represents a state action configuration for actions with two parameters.
/// </summary>
/// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
public interface IActionConfiguration<in T1, in T2>
{
    /// <summary>
    ///     Builds the action based on the configuration.
    /// </summary>
    /// <returns>The action model.</returns>
    internal IAction<T1, T2> Build();
}

/// <summary>
///     Represents a state action configuration for actions with three parameters.
/// </summary>
/// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
public interface IActionConfiguration<in T1, in T2, in T3>
{
    /// <summary>
    ///     Builds the action based on the configuration.
    /// </summary>
    /// <returns>The action model.</returns>
    internal IAction<T1, T2, T3> Build();
}

/// <summary>
///     Represents a state action configuration for actions with four parameters.
/// </summary>
/// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to the action.</typeparam>
public interface IActionConfiguration<in T1, in T2, in T3, in T4>
{
    /// <summary>
    ///     Builds the action based on the configuration.
    /// </summary>
    /// <returns>The action model.</returns>
    internal IAction<T1, T2, T3, T4> Build();
}
