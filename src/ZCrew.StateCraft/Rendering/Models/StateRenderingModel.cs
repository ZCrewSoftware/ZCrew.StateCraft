using System.Diagnostics.CodeAnalysis;

namespace ZCrew.StateCraft.Rendering.Models;

/// <summary>
///     The renderable view of a single state in a state machine. Captures both the underlying state value and the
///     identifier and display string that a renderer (Mermaid, GraphViz, etc.) needs to draw and reference the state.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
internal sealed class StateRenderingModel<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     Initializes a new <see cref="StateRenderingModel{TState, TTransition}"/>.
    /// </summary>
    /// <param name="state">The state value.</param>
    /// <param name="typeParameters">The type parameters of the state, in declaration order; empty for parameterless states.</param>
    /// <param name="identifier">The stable identifier used to reference the state from transitions.</param>
    /// <param name="descriptor">The human-readable display string for the state.</param>
    [SetsRequiredMembers]
    public StateRenderingModel(TState state, IReadOnlyList<Type> typeParameters, string identifier, string descriptor)
    {
        State = state;
        TypeParameters = typeParameters;
        Identifier = identifier;
        Descriptor = descriptor;
    }

    /// <summary>
    ///     The state value.
    /// </summary>
    public required TState State { get; init; }

    /// <summary>
    ///     The type parameters of the state.
    /// </summary>
    public required IReadOnlyList<Type> TypeParameters { get; init; }

    /// <summary>
    ///     The stable identifier used to reference this state from transitions. Built from the state value plus its type
    ///     parameters so that two parameterized states sharing the same underlying value but different parameter types
    ///     produce distinct ids.
    /// </summary>
    public required string Identifier { get; init; }

    /// <summary>
    ///     The human-readable display string for the state — the <see cref="object.ToString"/> form, which includes the
    ///     type parameters for parameterized states.
    /// </summary>
    public required string Descriptor { get; init; }
}
