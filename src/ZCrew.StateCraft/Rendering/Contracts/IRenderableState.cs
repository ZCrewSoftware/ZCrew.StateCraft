namespace ZCrew.StateCraft.Rendering.Contracts;

/// <summary>
///     Represents a type that refers to a state and can render an identifier to uniquely identify the state.
/// </summary>
internal interface IRenderableState
{
    /// <summary>
    ///     Return a unique identifier for a single state.
    /// </summary>
    /// <returns>The state identifier.</returns>
    string RenderStateIdentifier();
}
