using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Async;

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{Boolean}"/> that represents a parameterless
///     asynchronous predicate evaluated by the state machine.
/// </summary>
/// <remarks>
///     This wrapper exists to give conditions a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with conditions in the future without changing call sites.
/// </remarks>
/// <param name="Condition">The underlying asynchronous predicate that is evaluated when the condition runs.</param>
internal readonly record struct AsyncCondition(IAsyncFunc<bool> Condition)
{
    /// <summary>
    ///     Evaluates the wrapped condition.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to the boolean result of the condition.</returns>
    public Task<bool> Evaluate(CancellationToken token)
    {
        return Condition.InvokeAsync(token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{T, Boolean}"/> that represents an asynchronous
///     predicate accepting a single parameter.
/// </summary>
/// <remarks>
///     This wrapper exists to give conditions a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with conditions in the future without changing call sites.
/// </remarks>
/// <typeparam name="T">The type of the parameter passed to the condition.</typeparam>
/// <param name="Condition">The underlying asynchronous predicate that is evaluated when the condition runs.</param>
internal readonly record struct AsyncCondition<T>(IAsyncFunc<T, bool> Condition)
{
    /// <summary>
    ///     Evaluates the wrapped condition with the provided parameter.
    /// </summary>
    /// <param name="parameter">The parameter forwarded to the condition.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to the boolean result of the condition.</returns>
    public Task<bool> Evaluate(T parameter, CancellationToken token)
    {
        return Condition.InvokeAsync(parameter, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{T1, T2, Boolean}"/> that represents an asynchronous
///     predicate accepting two parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give conditions a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with conditions in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the condition.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the condition.</typeparam>
/// <param name="Condition">The underlying asynchronous predicate that is evaluated when the condition runs.</param>
internal readonly record struct AsyncCondition<T1, T2>(IAsyncFunc<T1, T2, bool> Condition)
{
    /// <summary>
    ///     Evaluates the wrapped condition with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the condition.</param>
    /// <param name="parameter2">The second parameter forwarded to the condition.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to the boolean result of the condition.</returns>
    public Task<bool> Evaluate(T1 parameter1, T2 parameter2, CancellationToken token)
    {
        return Condition.InvokeAsync(parameter1, parameter2, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{T1, T2, T3, Boolean}"/> that represents an
///     asynchronous predicate accepting three parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give conditions a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with conditions in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the condition.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the condition.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the condition.</typeparam>
/// <param name="Condition">The underlying asynchronous predicate that is evaluated when the condition runs.</param>
internal readonly record struct AsyncCondition<T1, T2, T3>(IAsyncFunc<T1, T2, T3, bool> Condition)
{
    /// <summary>
    ///     Evaluates the wrapped condition with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the condition.</param>
    /// <param name="parameter2">The second parameter forwarded to the condition.</param>
    /// <param name="parameter3">The third parameter forwarded to the condition.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to the boolean result of the condition.</returns>
    public Task<bool> Evaluate(T1 parameter1, T2 parameter2, T3 parameter3, CancellationToken token)
    {
        return Condition.InvokeAsync(parameter1, parameter2, parameter3, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{T1, T2, T3, T4, Boolean}"/> that represents an
///     asynchronous predicate accepting four parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give conditions a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with conditions in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the condition.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the condition.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the condition.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to the condition.</typeparam>
/// <param name="Condition">The underlying asynchronous predicate that is evaluated when the condition runs.</param>
internal readonly record struct AsyncCondition<T1, T2, T3, T4>(IAsyncFunc<T1, T2, T3, T4, bool> Condition)
{
    /// <summary>
    ///     Evaluates the wrapped condition with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the condition.</param>
    /// <param name="parameter2">The second parameter forwarded to the condition.</param>
    /// <param name="parameter3">The third parameter forwarded to the condition.</param>
    /// <param name="parameter4">The fourth parameter forwarded to the condition.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to the boolean result of the condition.</returns>
    public Task<bool> Evaluate(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, CancellationToken token)
    {
        return Condition.InvokeAsync(parameter1, parameter2, parameter3, parameter4, token);
    }
}
