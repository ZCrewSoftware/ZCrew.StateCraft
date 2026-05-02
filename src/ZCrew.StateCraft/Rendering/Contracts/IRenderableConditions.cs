namespace ZCrew.StateCraft.Rendering.Contracts;

/// <summary>
///     Represents a type that has meaningful information about conditional rendering components.
/// </summary>
internal interface IRenderableConditions
{
    /// <summary>
    ///     Return human-readable descriptors for each condition that gates the component, used to label the conditional
    ///     component when it is rendered.
    /// </summary>
    /// <returns>
    ///     One descriptor per condition, in the order the conditions were registered. Empty when the component is
    ///     unconditional.
    /// </returns>
    IEnumerable<string> RenderConditions();
}
