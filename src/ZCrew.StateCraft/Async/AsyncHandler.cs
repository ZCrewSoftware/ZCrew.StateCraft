using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Async;

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction"/> that represents a parameterless asynchronous
///     handler invoked by the state machine.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler(IAsyncAction Handler)
{
    /// <summary>
    ///     Invokes the wrapped handler.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(CancellationToken token)
    {
        return Handler.InvokeAsync(token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction{T}"/> that represents an asynchronous handler
///     accepting a single parameter.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <typeparam name="T">The type of the parameter passed to the handler.</typeparam>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler<T>(IAsyncAction<T> Handler)
{
    /// <summary>
    ///     Invokes the wrapped handler with the provided parameter.
    /// </summary>
    /// <param name="parameter">The parameter forwarded to the handler.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(T parameter, CancellationToken token)
    {
        return Handler.InvokeAsync(parameter, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction{T1, T2}"/> that represents an asynchronous handler
///     accepting two parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the handler.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the handler.</typeparam>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler<T1, T2>(IAsyncAction<T1, T2> Handler)
{
    /// <summary>
    ///     Invokes the wrapped handler with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the handler.</param>
    /// <param name="parameter2">The second parameter forwarded to the handler.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(T1 parameter1, T2 parameter2, CancellationToken token)
    {
        return Handler.InvokeAsync(parameter1, parameter2, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction{T1, T2, T3}"/> that represents an asynchronous
///     handler accepting three parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the handler.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the handler.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the handler.</typeparam>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler<T1, T2, T3>(IAsyncAction<T1, T2, T3> Handler)
{
    /// <summary>
    ///     Invokes the wrapped handler with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the handler.</param>
    /// <param name="parameter2">The second parameter forwarded to the handler.</param>
    /// <param name="parameter3">The third parameter forwarded to the handler.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(T1 parameter1, T2 parameter2, T3 parameter3, CancellationToken token)
    {
        return Handler.InvokeAsync(parameter1, parameter2, parameter3, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction{T1, T2, T3, T4}"/> that represents an asynchronous
///     handler accepting four parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the handler.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the handler.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the handler.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to the handler.</typeparam>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler<T1, T2, T3, T4>(IAsyncAction<T1, T2, T3, T4> Handler)
{
    /// <summary>
    ///     Invokes the wrapped handler with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the handler.</param>
    /// <param name="parameter2">The second parameter forwarded to the handler.</param>
    /// <param name="parameter3">The third parameter forwarded to the handler.</param>
    /// <param name="parameter4">The fourth parameter forwarded to the handler.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, CancellationToken token)
    {
        return Handler.InvokeAsync(parameter1, parameter2, parameter3, parameter4, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction{T1, T2, T3, T4, T5}"/> that represents an
///     asynchronous handler accepting five parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the handler.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the handler.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the handler.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to the handler.</typeparam>
/// <typeparam name="T5">The type of the fifth parameter passed to the handler.</typeparam>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler<T1, T2, T3, T4, T5>(IAsyncAction<T1, T2, T3, T4, T5> Handler)
{
    /// <summary>
    ///     Invokes the wrapped handler with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the handler.</param>
    /// <param name="parameter2">The second parameter forwarded to the handler.</param>
    /// <param name="parameter3">The third parameter forwarded to the handler.</param>
    /// <param name="parameter4">The fourth parameter forwarded to the handler.</param>
    /// <param name="parameter5">The fifth parameter forwarded to the handler.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        T5 parameter5,
        CancellationToken token
    )
    {
        return Handler.InvokeAsync(parameter1, parameter2, parameter3, parameter4, parameter5, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction{T1, T2, T3, T4, T5, T6}"/> that represents an
///     asynchronous handler accepting six parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the handler.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the handler.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the handler.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to the handler.</typeparam>
/// <typeparam name="T5">The type of the fifth parameter passed to the handler.</typeparam>
/// <typeparam name="T6">The type of the sixth parameter passed to the handler.</typeparam>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler<T1, T2, T3, T4, T5, T6>(IAsyncAction<T1, T2, T3, T4, T5, T6> Handler)
{
    /// <summary>
    ///     Invokes the wrapped handler with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the handler.</param>
    /// <param name="parameter2">The second parameter forwarded to the handler.</param>
    /// <param name="parameter3">The third parameter forwarded to the handler.</param>
    /// <param name="parameter4">The fourth parameter forwarded to the handler.</param>
    /// <param name="parameter5">The fifth parameter forwarded to the handler.</param>
    /// <param name="parameter6">The sixth parameter forwarded to the handler.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        T5 parameter5,
        T6 parameter6,
        CancellationToken token
    )
    {
        return Handler.InvokeAsync(parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction{T1, T2, T3, T4, T5, T6, T7}"/> that represents an
///     asynchronous handler accepting seven parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the handler.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the handler.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the handler.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to the handler.</typeparam>
/// <typeparam name="T5">The type of the fifth parameter passed to the handler.</typeparam>
/// <typeparam name="T6">The type of the sixth parameter passed to the handler.</typeparam>
/// <typeparam name="T7">The type of the seventh parameter passed to the handler.</typeparam>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler<T1, T2, T3, T4, T5, T6, T7>(
    IAsyncAction<T1, T2, T3, T4, T5, T6, T7> Handler
)
{
    /// <summary>
    ///     Invokes the wrapped handler with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the handler.</param>
    /// <param name="parameter2">The second parameter forwarded to the handler.</param>
    /// <param name="parameter3">The third parameter forwarded to the handler.</param>
    /// <param name="parameter4">The fourth parameter forwarded to the handler.</param>
    /// <param name="parameter5">The fifth parameter forwarded to the handler.</param>
    /// <param name="parameter6">The sixth parameter forwarded to the handler.</param>
    /// <param name="parameter7">The seventh parameter forwarded to the handler.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        T5 parameter5,
        T6 parameter6,
        T7 parameter7,
        CancellationToken token
    )
    {
        return Handler.InvokeAsync(
            parameter1,
            parameter2,
            parameter3,
            parameter4,
            parameter5,
            parameter6,
            parameter7,
            token
        );
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncAction{T1, T2, T3, T4, T5, T6, T7, T8}"/> that represents
///     an asynchronous handler accepting eight parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give handlers a distinct nominal type rather than relying on bare delegate types,
///     which allows additional metadata to be associated with handlers in the future without changing call sites.
/// </remarks>
/// <typeparam name="T1">The type of the first parameter passed to the handler.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the handler.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the handler.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to the handler.</typeparam>
/// <typeparam name="T5">The type of the fifth parameter passed to the handler.</typeparam>
/// <typeparam name="T6">The type of the sixth parameter passed to the handler.</typeparam>
/// <typeparam name="T7">The type of the seventh parameter passed to the handler.</typeparam>
/// <typeparam name="T8">The type of the eighth parameter passed to the handler.</typeparam>
/// <param name="Handler">The underlying asynchronous action that is invoked when the handler runs.</param>
internal readonly record struct AsyncHandler<T1, T2, T3, T4, T5, T6, T7, T8>(
    IAsyncAction<T1, T2, T3, T4, T5, T6, T7, T8> Handler
)
{
    /// <summary>
    ///     Invokes the wrapped handler with the provided parameters.
    /// </summary>
    /// <param name="parameter1">The first parameter forwarded to the handler.</param>
    /// <param name="parameter2">The second parameter forwarded to the handler.</param>
    /// <param name="parameter3">The third parameter forwarded to the handler.</param>
    /// <param name="parameter4">The fourth parameter forwarded to the handler.</param>
    /// <param name="parameter5">The fifth parameter forwarded to the handler.</param>
    /// <param name="parameter6">The sixth parameter forwarded to the handler.</param>
    /// <param name="parameter7">The seventh parameter forwarded to the handler.</param>
    /// <param name="parameter8">The eighth parameter forwarded to the handler.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the handler finishes executing.</returns>
    public Task Invoke(
        T1 parameter1,
        T2 parameter2,
        T3 parameter3,
        T4 parameter4,
        T5 parameter5,
        T6 parameter6,
        T7 parameter7,
        T8 parameter8,
        CancellationToken token
    )
    {
        return Handler.InvokeAsync(
            parameter1,
            parameter2,
            parameter3,
            parameter4,
            parameter5,
            parameter6,
            parameter7,
            parameter8,
            token
        );
    }
}
