using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents common functionality to describe a parameterized state action configuration regardless of the
///     semantics of the action.
/// </summary>
/// <typeparam name="T">The type of the parameter passed to the action.</typeparam>
public interface IParameterizedActionConfiguration<in T>
{
    /// <summary>
    ///     Builds the action based on the configuration.
    /// </summary>
    /// <returns>The action model.</returns>
    internal IParameterizedAction<T> Build();
}
