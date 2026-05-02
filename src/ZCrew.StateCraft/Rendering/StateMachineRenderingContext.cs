using ZCrew.StateCraft.Rendering.Contracts;
using ZCrew.StateCraft.Rendering.Models;

namespace ZCrew.StateCraft.Rendering;

/// <summary>
///     A mutable accumulator that <see cref="IRenderable{TState, TTransition}"/> implementations write into while a state
///     machine configuration tree is walked. Once populated, the collected models describe the full structure that a
///     renderer needs to produce a diagram.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
/// <typeparam name="TTransition">The type of the transition.</typeparam>
internal sealed class StateMachineRenderingContext<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    /// <summary>
    ///     The top-level model describing the state machine itself. Populated by the configuration during
    ///     <see cref="AddToRenderingContext(IStateMachineConfiguration{TState, TTransition})"/>; <see langword="null"/>
    ///     until then.
    /// </summary>
    public StateMachineRenderingModel<TState, TTransition>? StateMachine { get; set; }

    /// <summary>
    ///     The collected state models, in the order they were added by the configuration walk.
    /// </summary>
    public List<StateRenderingModel<TState, TTransition>> States { get; } = [];

    /// <summary>
    ///     The collected transition models, in the order they were added by the configuration walk.
    /// </summary>
    public List<TransitionRenderingModel<TState, TTransition>> Transitions { get; } = [];

    /// <summary>
    ///     Walks the supplied configuration and populates this context. The state machine itself is added first, then
    ///     every state, then every transition — that order ensures all <see cref="States"/> are present before any
    ///     transition model is built, which is required to support one-to-many, many-to-one, and many-to-many transition
    ///     shapes.
    /// </summary>
    /// <param name="stateMachineConfiguration">The configuration to render.</param>
    public void AddToRenderingContext(IStateMachineConfiguration<TState, TTransition> stateMachineConfiguration)
    {
        if (stateMachineConfiguration is IRenderable<TState, TTransition> renderableConfiguration)
        {
            renderableConfiguration.AddToRenderingContext(this);
        }

        var states = stateMachineConfiguration.States;
        var transitions = stateMachineConfiguration.States.SelectMany(state => state.Transitions);

        // To accomodate transitions that are one-to-many, many-to-one, or many-to-many, add the states first and then
        // the transitions. This ensures all states are present when creating the transition models
        foreach (var state in states)
        {
            if (state is IRenderable<TState, TTransition> renderable)
            {
                renderable.AddToRenderingContext(this);
            }
        }
        foreach (var transition in transitions)
        {
            if (transition is IRenderable<TState, TTransition> renderable)
            {
                renderable.AddToRenderingContext(this);
            }
        }
    }
}
