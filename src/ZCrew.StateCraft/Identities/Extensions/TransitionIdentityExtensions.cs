using System.Text;

namespace ZCrew.StateCraft.Identities.Extensions;

/// <summary>
///     Display helpers for <see cref="ITransitionIdentity{TTransition}"/>. The single home for rendering a transition
///     token and a full transition edge so every layer forwards here instead of duplicating the logic.
/// </summary>
internal static class TransitionIdentityExtensions
{
    /// <summary>
    ///     Renders the transition token: just the value for a parameterless transition (e.g. <c>Go</c>), or the value
    ///     followed by its parameter types (e.g. <c>Go&lt;int&gt;</c>).
    /// </summary>
    /// <param name="transition">The transition identity to render.</param>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <returns>A non-<see langword="null"/> display string for the transition token.</returns>
    public static string ToDisplayString<TTransition>(this ITransitionIdentity<TTransition> transition)
        where TTransition : notnull
    {
        if (transition.TransitionParameterTypes.Count == 0)
        {
            return $"{transition.TransitionValue}";
        }

        var builder = new StringBuilder();
        builder.Append(transition.TransitionValue).Append('<');

        for (var i = 0; i < transition.TransitionParameterTypes.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(", ");
            }
            builder.Append(transition.TransitionParameterTypes[i].FriendlyName);
        }

        return builder.Append('>').ToString();
    }

    /// <summary>
    ///     Renders a transition edge from a single source state to a target state, e.g.
    ///     <c>Go(A) → B</c>, or <c>Go(A)  ↩</c> when the transition is reentrant (source and target share identity).
    /// </summary>
    /// <param name="transition">The transition identity to render.</param>
    /// <param name="source">The single source state of the transition.</param>
    /// <param name="target">The target state of the transition.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <returns>A non-<see langword="null"/> display string for the edge.</returns>
    public static string RenderFromOneToOne<TState, TTransition>(
        this ITransitionIdentity<TTransition> transition,
        IStateIdentity<TState> source,
        IStateIdentity<TState> target
    )
        where TState : notnull
        where TTransition : notnull
    {
        return source.Matches(target)
            ? $"{transition.ToDisplayString()}({source.ToDisplayString()}) ↩"
            : $"{transition.ToDisplayString()}({source.ToDisplayString()}) → {target.ToDisplayString()}";
    }

    /// <summary>
    ///     Renders a transition edge whose source is every state (optionally minus an excluded set) to a target state,
    ///     e.g. <c>Go(Any State) → B</c> or <c>Go(Any State Except: A) → B</c>.
    /// </summary>
    /// <param name="transition">The transition identity to render.</param>
    /// <param name="excluded">The states excluded from the inverted source set. Empty means every state.</param>
    /// <param name="target">The target state of the transition.</param>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TTransition">The transition type.</typeparam>
    /// <returns>A non-<see langword="null"/> display string for the edge.</returns>
    public static string RenderFromAnyToOne<TState, TTransition>(
        this ITransitionIdentity<TTransition> transition,
        IReadOnlyList<IStateIdentity<TState>> excluded,
        IStateIdentity<TState> target
    )
        where TState : notnull
        where TTransition : notnull
    {
        if (excluded.Count == 0)
        {
            return $"{transition.ToDisplayString()}(Any State) → {target.ToDisplayString()}";
        }

        var excludedLabels = string.Join(", ", excluded.Select(state => state.ToDisplayString()));
        return $"{transition.ToDisplayString()}(Any State Except: {excludedLabels}) → {target.ToDisplayString()}";
    }
}
