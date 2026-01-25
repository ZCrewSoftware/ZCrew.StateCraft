using ZCrew.StateCraft.StateMachines.Contracts;

namespace ZCrew.StateCraft;

/// <summary>
///     Extension methods for <see cref="ITransition{TState, TTransition}"/>.
/// </summary>
internal static class TransitionExtensions
{
    extension<TState, TTransition>(ITransition<TState, TTransition> transition)
        where TState : notnull
        where TTransition : notnull
    {
        /// <summary>
        ///     Gets the state machine that owns the previous state of this transition.
        /// </summary>
        internal IStateMachine<TState, TTransition> StateMachine => transition.PreviousState.StateMachine;
    }
}
