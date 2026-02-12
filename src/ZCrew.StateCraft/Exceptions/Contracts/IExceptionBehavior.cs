namespace ZCrew.StateCraft;

/// <summary>
///     Defines exception handling behavior for state machine call sites. Each method wraps a
///     specific handler invocation, giving implementations control over how exceptions are
///     intercepted, logged, suppressed, or propagated.
/// </summary>
/// <seealso cref="ZCrew.StateCraft.Exceptions.DefaultExceptionBehavior"/>
public interface IExceptionBehavior
{
    /// <summary>
    ///     Wraps a state's <c>OnEntry</c> handler with exception handling.
    /// </summary>
    /// <param name="handler">The entry handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes.</returns>
    Task CallOnEntry(Func<CancellationToken, Task> handler, CancellationToken token = default);

    /// <summary>
    ///     Wraps a state's <c>OnExit</c> handler with exception handling.
    /// </summary>
    /// <param name="handler">The exit handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes.</returns>
    Task CallOnExit(Func<CancellationToken, Task> handler, CancellationToken token = default);

    /// <summary>
    ///     Wraps the state machine's <c>OnStateChange</c> handler with exception handling.
    /// </summary>
    /// <param name="handler">The state change handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes.</returns>
    Task CallOnStateChange(Func<CancellationToken, Task> handler, CancellationToken token = default);

    /// <summary>
    ///     Wraps a state's <c>OnActivate</c> handler with exception handling.
    /// </summary>
    /// <param name="handler">The activation handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes.</returns>
    Task CallOnActivate(Func<CancellationToken, Task> handler, CancellationToken token = default);

    /// <summary>
    ///     Wraps a state's <c>OnDeactivate</c> handler with exception handling.
    /// </summary>
    /// <param name="handler">The deactivation handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes.</returns>
    Task CallOnDeactivate(Func<CancellationToken, Task> handler, CancellationToken token = default);

    /// <summary>
    ///     Wraps a transition's condition evaluation with exception handling.
    /// </summary>
    /// <param name="handler">The condition handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task whose result is <see langword="true"/> if the condition is satisfied;
    ///     otherwise <see langword="false"/>.
    /// </returns>
    Task<bool> CallCondition(Func<CancellationToken, Task<bool>> handler, CancellationToken token = default);

    /// <summary>
    ///     Wraps a mapped transition's mapping function with exception handling.
    /// </summary>
    /// <param name="handler">The mapping handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes.</returns>
    Task CallMap(Func<CancellationToken, Task> handler, CancellationToken token = default);

    /// <summary>
    ///     Wraps a state's action invocation with exception handling.
    /// </summary>
    /// <param name="handler">The action handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes.</returns>
    Task CallAction(Func<CancellationToken, Task> handler, CancellationToken token = default);

    /// <summary>
    ///     Wraps a trigger's invocation with exception handling.
    /// </summary>
    /// <param name="handler">The trigger handler to invoke.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes.</returns>
    Task CallTrigger(Func<CancellationToken, Task> handler, CancellationToken token = default);
}
