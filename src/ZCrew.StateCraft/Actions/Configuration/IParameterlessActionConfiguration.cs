using ZCrew.StateCraft.Actions.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Represents common functionality to describe a parameterless state action configuration regardless of the
///     semantics of the action.
/// </summary>
public interface IParameterlessActionConfiguration
{
    /// <summary>
    ///     Builds the action based on the configuration.
    /// </summary>
    /// <returns>The action model.</returns>
    internal IParameterlessAction Build();
}
