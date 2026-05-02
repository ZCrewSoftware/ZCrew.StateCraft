namespace ZCrew.StateCraft.Rendering.Contracts;

/// <summary>
///     Represents a type that has meaningful information to add to a rendering context.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
internal interface IRenderable<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Add information to the rendering context to perform rendering, such as exporting to a mermaid diagram.
    /// </summary>
    /// <param name="context">The rendering context to add to.</param>
    void AddToRenderingContext(StateMachineRenderingContext<TState, TTransition> context);
}
