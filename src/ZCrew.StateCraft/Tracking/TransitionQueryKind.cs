namespace ZCrew.StateCraft.Tracking;

/// <summary>
///     Identifies which state machine entry point asked for a transition.
/// </summary>
internal enum TransitionQueryKind
{
    /// <summary>
    ///     The request came from <c>Transition</c>, which throws when no transition matches.
    /// </summary>
    Transition,

    /// <summary>
    ///     The request came from <c>TryTransition</c>, which returns <see langword="false"/> when no transition
    ///     matches.
    /// </summary>
    TryTransition,

    /// <summary>
    ///     The request came from <c>CanTransition</c>. Conditions and mapping functions run, but no transition is
    ///     performed.
    /// </summary>
    CanTransition,
}
