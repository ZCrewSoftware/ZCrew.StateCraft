namespace ZCrew.StateCraft.Tracking;

/// <summary>
///     Why a candidate transition was skipped while looking up the transition to apply.
/// </summary>
internal enum TransitionSkippedReason
{
    /// <summary>
    ///     The candidate is registered for a different transition value.
    /// </summary>
    TransitionValueMismatch,

    /// <summary>
    ///     The supplied parameters are not assignable to the candidate's declared parameter types.
    /// </summary>
    ParameterTypeMismatch,

    /// <summary>
    ///     The candidate's conditions evaluated to <see langword="false"/>.
    /// </summary>
    ConditionFailed,
}
