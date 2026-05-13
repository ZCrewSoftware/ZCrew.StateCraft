using ZCrew.Extensions.Tasks;

namespace ZCrew.StateCraft.Async;

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TIn, TTuple}"/> producing a two-element value
///     tuple that represents an asynchronous mapping function used by the state machine during a parameter-mapped
///     transition that emits two output parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give mapping functions a distinct nominal type rather than relying on bare delegate
///     types, which allows additional metadata to be associated with mappings in the future without changing call
///     sites.
/// </remarks>
/// <typeparam name="TIn">The type of the input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
/// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
/// <param name="Map">The underlying asynchronous function that is invoked when the mapping runs.</param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncMapValueTuple2<TIn, TOut1, TOut2>(
    IAsyncFunc<TIn, (TOut1, TOut2)> Map,
    string? Descriptor
)
{
    /// <summary>
    ///     Invokes the wrapped mapping function with the provided input parameter.
    /// </summary>
    /// <param name="parameter">The input parameter forwarded to the mapping function.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the mapped output value tuple.</returns>
    public Task<(TOut1, TOut2)> Invoke(TIn parameter, CancellationToken token)
    {
        return Map.InvokeAsync(parameter, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TIn1, TIn2, TTuple}"/> producing a two-element
///     value tuple that represents an asynchronous mapping function accepting two input parameters used by the
///     state machine during a parameter-mapped transition that emits two output parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give mapping functions a distinct nominal type rather than relying on bare delegate
///     types, which allows additional metadata to be associated with mappings in the future without changing call
///     sites.
/// </remarks>
/// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
/// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
/// <param name="Map">The underlying asynchronous function that is invoked when the mapping runs.</param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncMapValueTuple2<TIn1, TIn2, TOut1, TOut2>(
    IAsyncFunc<TIn1, TIn2, (TOut1, TOut2)> Map,
    string? Descriptor
)
{
    /// <summary>
    ///     Invokes the wrapped mapping function with the provided input parameters.
    /// </summary>
    /// <param name="parameter1">The first input parameter forwarded to the mapping function.</param>
    /// <param name="parameter2">The second input parameter forwarded to the mapping function.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the mapped output value tuple.</returns>
    public Task<(TOut1, TOut2)> Invoke(TIn1 parameter1, TIn2 parameter2, CancellationToken token)
    {
        return Map.InvokeAsync(parameter1, parameter2, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TIn1, TIn2, TIn3, TTuple}"/> producing a
///     two-element value tuple that represents an asynchronous mapping function accepting three input parameters
///     used by the state machine during a parameter-mapped transition that emits two output parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give mapping functions a distinct nominal type rather than relying on bare delegate
///     types, which allows additional metadata to be associated with mappings in the future without changing call
///     sites.
/// </remarks>
/// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
/// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
/// <param name="Map">The underlying asynchronous function that is invoked when the mapping runs.</param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncMapValueTuple2<TIn1, TIn2, TIn3, TOut1, TOut2>(
    IAsyncFunc<TIn1, TIn2, TIn3, (TOut1, TOut2)> Map,
    string? Descriptor
)
{
    /// <summary>
    ///     Invokes the wrapped mapping function with the provided input parameters.
    /// </summary>
    /// <param name="parameter1">The first input parameter forwarded to the mapping function.</param>
    /// <param name="parameter2">The second input parameter forwarded to the mapping function.</param>
    /// <param name="parameter3">The third input parameter forwarded to the mapping function.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the mapped output value tuple.</returns>
    public Task<(TOut1, TOut2)> Invoke(TIn1 parameter1, TIn2 parameter2, TIn3 parameter3, CancellationToken token)
    {
        return Map.InvokeAsync(parameter1, parameter2, parameter3, token);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TIn1, TIn2, TIn3, TIn4, TTuple}"/> producing a
///     two-element value tuple that represents an asynchronous mapping function accepting four input parameters
///     used by the state machine during a parameter-mapped transition that emits two output parameters.
/// </summary>
/// <remarks>
///     This wrapper exists to give mapping functions a distinct nominal type rather than relying on bare delegate
///     types, which allows additional metadata to be associated with mappings in the future without changing call
///     sites.
/// </remarks>
/// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn4">The type of the fourth input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
/// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
/// <param name="Map">The underlying asynchronous function that is invoked when the mapping runs.</param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncMapValueTuple2<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2>(
    IAsyncFunc<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2)> Map,
    string? Descriptor
)
{
    /// <summary>
    ///     Invokes the wrapped mapping function with the provided input parameters.
    /// </summary>
    /// <param name="parameter1">The first input parameter forwarded to the mapping function.</param>
    /// <param name="parameter2">The second input parameter forwarded to the mapping function.</param>
    /// <param name="parameter3">The third input parameter forwarded to the mapping function.</param>
    /// <param name="parameter4">The fourth input parameter forwarded to the mapping function.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the mapped output value tuple.</returns>
    public Task<(TOut1, TOut2)> Invoke(
        TIn1 parameter1,
        TIn2 parameter2,
        TIn3 parameter3,
        TIn4 parameter4,
        CancellationToken token
    )
    {
        return Map.InvokeAsync(parameter1, parameter2, parameter3, parameter4, token);
    }
}
