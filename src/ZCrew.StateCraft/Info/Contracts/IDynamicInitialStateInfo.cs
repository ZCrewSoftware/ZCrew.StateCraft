namespace ZCrew.StateCraft;

/// <summary>
///     Introspection metadata for an initial state whose value is fetched by a delegate when the state machine is
///     activated. The state value and any parameters are not known at configuration time.
/// </summary>
/// <remarks>
///     Because the value is only known at activation time, no build-time validation can verify it matches a
///     configured state. A provider that returns a state which is not configured surfaces as a failure when the
///     state machine is activated.
/// </remarks>
/// <typeparam name="TState">
///     The state type. This should be an <see langword="enum"/> type or it should be an equatable type so the state
///     machine behaves as expected.
/// </typeparam>
public interface IDynamicInitialStateInfo<TState> : IInitialStateInfo<TState>
    where TState : notnull
{
    /// <summary>
    ///     A human-readable label for the state-provider delegate, typically the caller's expression captured at
    ///     configuration time via <see cref="System.Runtime.CompilerServices.CallerArgumentExpressionAttribute"/>.
    ///     <see langword="null"/> when no descriptor was supplied.
    /// </summary>
    string? Descriptor { get; }
}
