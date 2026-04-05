using System.Diagnostics.CodeAnalysis;

namespace ZCrew.StateCraft.Validation.Models;

/// <summary>
///     Model representing a <see cref="ITransitionConfiguration{TState,TTransition}"/> used for validation.
/// </summary>
internal class TransitionValidationModel<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    [SetsRequiredMembers]
    public TransitionValidationModel(
        TState previousStateValue,
        TTransition transitionValue,
        TState nextStateValue,
        IReadOnlyList<Type> previousStateTypeParameters,
        IReadOnlyList<Type> transitionTypeParameters,
        IReadOnlyList<Type> nextStateTypeParameters,
        bool isConditional
    )
    {
        PreviousStateValue = previousStateValue;
        TransitionValue = transitionValue;
        NextStateValue = nextStateValue;
        PreviousStateTypeParameters = previousStateTypeParameters;
        TransitionTypeParameters = transitionTypeParameters;
        NextStateTypeParameters = nextStateTypeParameters;
        IsConditional = isConditional;
    }

    /// <summary>
    ///     The previous state value.
    /// </summary>
    public required TState PreviousStateValue { get; init; }

    /// <summary>
    ///     The transition value.
    /// </summary>
    public required TTransition TransitionValue { get; init; }

    /// <summary>
    ///     The next state value.
    /// </summary>
    public required TState NextStateValue { get; init; }

    /// <summary>
    ///     The type parameters of the previous state. Empty if the previous state has no parameters.
    /// </summary>
    public required IReadOnlyList<Type> PreviousStateTypeParameters { get; init; }

    /// <summary>
    ///     The type parameters of the transition. Empty if the transition can be invoked without providing a parameter.
    /// </summary>
    public required IReadOnlyList<Type> TransitionTypeParameters { get; init; }

    /// <summary>
    ///     The type parameters of the next state. Empty if the next state has no parameters.
    /// </summary>
    public required IReadOnlyList<Type> NextStateTypeParameters { get; init; }

    /// <summary>
    ///     Indicates whether the transition is conditional. A conditional transition has one or more conditions that
    ///     must be satisfied for the transition to be taken.
    /// </summary>
    public required bool IsConditional { get; init; }

    /// <inheritdoc />
    public override string ToString()
    {
        var previousState =
            PreviousStateTypeParameters.Count > 0
                ? $"{PreviousStateValue}<{string.Join(", ", PreviousStateTypeParameters.Select(type => type.FriendlyName))}>"
                : $"{PreviousStateValue}";
        if (
            PreviousStateValue.Equals(NextStateValue)
            && PreviousStateTypeParameters.SequenceEqual(NextStateTypeParameters)
        )
        {
            return $"{TransitionValue}({previousState}) ↩";
        }

        var nextState =
            NextStateTypeParameters.Count > 0
                ? $"{NextStateValue}<{string.Join(", ", NextStateTypeParameters.Select(type => type.FriendlyName))}>"
                : $"{NextStateValue}";
        return $"{TransitionValue}({previousState}) → {nextState}";
    }
}
