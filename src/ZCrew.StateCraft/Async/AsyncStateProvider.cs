using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Async;

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TState}"/> that provides the initial state value.
/// </summary>
/// <remarks>
///     This wrapper exists to give the initial state provider a distinct nominal type rather than relying on bare
///     delegate types, which allows additional metadata to be associated with the initial state provider in the future
///     without changing call sites.
/// </remarks>
/// <typeparam name="TState">The type of the initial state value.</typeparam>
/// <param name="StateProvider">
///     The underlying asynchronous function that is invoked when the state machine is activated.
/// </param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncStateProvider<TState>(IAsyncFunc<TState> StateProvider, string? Descriptor)
    where TState : notnull
{
    /// <summary>
    ///     Invokes the wrapped state provider.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the initial state value.</returns>
    public Task<TState> Evaluate(CancellationToken token)
    {
        return StateProvider.InvokeAsync(token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TTuple}"/> that provides the initial state value and
///     parameter.
/// </summary>
/// <remarks>
///     This wrapper exists to give the initial state provider a distinct nominal type rather than relying on bare
///     delegate types, which allows additional metadata to be associated with the initial state provider in the future
///     without changing call sites.
/// </remarks>
/// <typeparam name="TState">The type of the initial state value.</typeparam>
/// <typeparam name="T">The type of the initial parameter.</typeparam>
/// <param name="StateProvider">
///     The underlying asynchronous function that is invoked when the state machine is activated.
/// </param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncStateProvider<TState, T>(IAsyncFunc<(TState, T)> StateProvider, string? Descriptor)
    where TState : notnull
{
    /// <summary>
    ///     Invokes the wrapped state provider.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the initial state value and parameter.</returns>
    public Task<(TState, T)> Evaluate(CancellationToken token)
    {
        return StateProvider.InvokeAsync(token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TTuple}"/> that provides the initial state value and
///     parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give the initial state provider a distinct nominal type rather than relying on bare
///     delegate types, which allows additional metadata to be associated with the initial state provider in the future
///     without changing call sites.
/// </remarks>
/// <typeparam name="TState">The type of the initial state value.</typeparam>
/// <typeparam name="T1">The type of the first initial parameter.</typeparam>
/// <typeparam name="T2">The type of the second initial parameter.</typeparam>
/// <param name="StateProvider">
///     The underlying asynchronous function that is invoked when the state machine is activated.
/// </param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncStateProvider<TState, T1, T2>(
    IAsyncFunc<(TState, T1, T2)> StateProvider,
    string? Descriptor
)
    where TState : notnull
{
    /// <summary>
    ///     Invokes the wrapped state provider.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the initial state value and parameters.</returns>
    public Task<(TState, T1, T2)> Evaluate(CancellationToken token)
    {
        return StateProvider.InvokeAsync(token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TTuple}"/> that provides the initial state value and
///     parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give the initial state provider a distinct nominal type rather than relying on bare
///     delegate types, which allows additional metadata to be associated with the initial state provider in the future
///     without changing call sites.
/// </remarks>
/// <typeparam name="TState">The type of the initial state value.</typeparam>
/// <typeparam name="T1">The type of the first initial parameter.</typeparam>
/// <typeparam name="T2">The type of the second initial parameter.</typeparam>
/// <typeparam name="T3">The type of the third initial parameter.</typeparam>
/// <param name="StateProvider">
///     The underlying asynchronous function that is invoked when the state machine is activated.
/// </param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncStateProvider<TState, T1, T2, T3>(
    IAsyncFunc<(TState, T1, T2, T3)> StateProvider,
    string? Descriptor
)
    where TState : notnull
{
    /// <summary>
    ///     Invokes the wrapped state provider.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the initial state value and parameters.</returns>
    public Task<(TState, T1, T2, T3)> Evaluate(CancellationToken token)
    {
        return StateProvider.InvokeAsync(token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TTuple}"/> that provides the initial state value and
///     parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give the initial state provider a distinct nominal type rather than relying on bare
///     delegate types, which allows additional metadata to be associated with the initial state provider in the future
///     without changing call sites.
/// </remarks>
/// <typeparam name="TState">The type of the initial state value.</typeparam>
/// <typeparam name="T1">The type of the first initial parameter.</typeparam>
/// <typeparam name="T2">The type of the second initial parameter.</typeparam>
/// <typeparam name="T3">The type of the third initial parameter.</typeparam>
/// <typeparam name="T4">The type of the fourth initial parameter.</typeparam>
/// <param name="StateProvider">
///     The underlying asynchronous function that is invoked when the state machine is activated.
/// </param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncStateProvider<TState, T1, T2, T3, T4>(
    IAsyncFunc<(TState, T1, T2, T3, T4)> StateProvider,
    string? Descriptor
)
    where TState : notnull
{
    /// <summary>
    ///     Invokes the wrapped state provider.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the initial state value and parameters.</returns>
    public Task<(TState, T1, T2, T3, T4)> Evaluate(CancellationToken token)
    {
        return StateProvider.InvokeAsync(token);
    }
}
