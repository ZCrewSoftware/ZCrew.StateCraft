using System.Diagnostics.CodeAnalysis;
using ZCrew.StateCraft.Extensions;

namespace ZCrew.StateCraft.Validation.Models;

/// <summary>
///     Model representing a <see cref="IStateConfiguration{TState,TTransition}"/> used for validation.
/// </summary>
internal class StateValidationModel<TState, TTransition>
    where TState : notnull
    where TTransition : notnull
{
    [SetsRequiredMembers]
    public StateValidationModel(TState state, IReadOnlyList<Type> typeParameters)
    {
        State = state;
        TypeParameters = typeParameters;
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
    ///     Whether this state would be assignable from a state composed of the <paramref name="state"/> value and
    ///     <paramref name="typeParameters"/> types.
    /// </summary>
    /// <param name="state">The state value.</param>
    /// <param name="typeParameters">The type parameters</param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="state"/> and <paramref name="typeParameters"/> are assignable
    ///     to this state.
    /// </returns>
    public bool IsAssignableFrom(TState state, IReadOnlyList<Type> typeParameters)
    {
        if (!EqualityComparer<TState>.Default.Equals(state, State))
        {
            return false;
        }

        return TypeParameters.IsAssignableFrom(typeParameters);
    }

    /// <summary>
    ///     Whether the state matches the transition's previous state.
    /// </summary>
    /// <param name="transition">The transition to evaluate the previous state of.</param>
    /// <returns><see langword="true"/> if the previous state is assignable to this state.</returns>
    public bool IsAssignableFromPreviousState(TransitionValidationModel<TState, TTransition> transition)
    {
        return IsAssignableFrom(transition.PreviousStateValue, transition.PreviousStateTypeParameters);
    }

    /// <summary>
    ///     Whether the state matches the transition's next state.
    /// </summary>
    /// <param name="transition">The transition to evaluate the next state of.</param>
    /// <returns><see langword="true"/> if the next state is assignable to this state.</returns>
    public bool IsAssignableFromNextState(TransitionValidationModel<TState, TTransition> transition)
    {
        return IsAssignableFrom(transition.NextStateValue, transition.NextStateTypeParameters);
    }

    /// <summary>
    ///     Whether this state is equal to the <paramref name="other"/> state.
    /// </summary>
    /// <param name="other">The other state.</param>
    /// <returns><see langword="true"/> if this state is equal to the <paramref name="other"/> state.</returns>
    public bool Equals(StateValidationModel<TState, TTransition> other)
    {
        if (!EqualityComparer<TState>.Default.Equals(other.State, State))
        {
            return false;
        }

        return TypeParameters.SequenceEqual(other.TypeParameters);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((StateValidationModel<TState, TTransition>)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(State);
        foreach (var typeParameter in TypeParameters)
        {
            hashCode.Add(typeParameter);
        }
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return TypeParameters.Count > 0
            ? $"{State}<{string.Join(", ", TypeParameters.Select(type => type.FriendlyName))}>"
            : $"{State}";
    }
}
