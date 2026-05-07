using System.Diagnostics.CodeAnalysis;

namespace ZCrew.StateCraft.Rendering.Models;

/// <summary>
///     The renderable view of a single transition between two states.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
internal sealed class TransitionRenderingModel<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Initializes a new <see cref="TransitionRenderingModel{TState, TTransition}"/>.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    /// <param name="nextState">The next state.</param>
    /// <param name="descriptor">The display label for the transition.</param>
    /// <param name="conditions">The descriptors of the conditions that gate the transition.</param>
    [SetsRequiredMembers]
    public TransitionRenderingModel(
        string previousState,
        string nextState,
        string descriptor,
        IReadOnlyList<string> conditions
    )
    {
        PreviousState = previousState;
        NextState = nextState;
        Descriptor = descriptor;
        Conditions = conditions;
    }

    /// <summary>
    ///     The previous state that is being transitioned from.
    /// </summary>
    public required string PreviousState { get; init; }

    /// <summary>
    ///     The next state that is being transitioned to.
    /// </summary>
    public required string NextState { get; init; }

    /// <summary>
    ///     The display label for the transition — typically the <see cref="object.ToString"/> form of the trigger value.
    /// </summary>
    public required string Descriptor { get; init; }

    /// <summary>
    ///     The descriptors of the conditions that gate the transition, gathered in order from the previous-state and
    ///     next-state configurations. Empty when the transition is unconditional.
    /// </summary>
    public required IReadOnlyList<string> Conditions { get; init; }
}
