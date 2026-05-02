using System.Diagnostics.CodeAnalysis;

namespace ZCrew.StateCraft.Rendering.Models;

/// <summary>
///     The top-level rendering description of a state machine. Sits alongside the per-state and per-transition models in
///     a <see cref="StateMachineRenderingContext{TState, TTransition}"/> and supplies the labelling for the diagram as a
///     whole.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
internal sealed class StateMachineRenderingModel<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Initializes a new <see cref="StateMachineRenderingModel{TState, TTransition}"/>.
    /// </summary>
    /// <param name="descriptor">The human-readable display string for the state machine itself.</param>
    [SetsRequiredMembers]
    public StateMachineRenderingModel(string descriptor)
    {
        Descriptor = descriptor;
    }

    /// <summary>
    ///     The display string for the state machine itself, used for example as the title of the rendered diagram.
    /// </summary>
    public required string Descriptor { get; init; }
}
