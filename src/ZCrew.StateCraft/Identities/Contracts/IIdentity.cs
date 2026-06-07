using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     A way to uniquely identify components of a <see cref="IStateMachine{TState,TTransition}"/>, build
///     <see cref="string"/> representations, or apply common logic.
/// </summary>
public interface IIdentity
{
    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    /// <remarks>
    ///     <see cref="Object.ToString"/> is nullable, this just forces a non-<see langword="null"/> value.
    /// </remarks>
    string ToString();
}
