using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Info;

namespace ZCrew.StateCraft.Async;

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TIn, TOut}"/> that represents an asynchronous
///     mapping function used by the state machine during a parameter-mapped transition.
/// </summary>
/// <remarks>
///     This wrapper exists to give mapping functions a distinct nominal type rather than relying on bare delegate
///     types, which allows additional metadata to be associated with mappings in the future without changing call
///     sites.
/// </remarks>
/// <typeparam name="TIn">The type of the input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TOut">The type of the output parameter produced by the mapping function.</typeparam>
/// <param name="Map">The underlying asynchronous function that is invoked when the mapping runs.</param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncMap<TIn, TOut>(IAsyncFunc<TIn, TOut> Map, string? Descriptor)
{
    /// <summary>
    ///     Invokes the wrapped mapping function with the provided input parameter.
    /// </summary>
    /// <param name="parameter">The input parameter forwarded to the mapping function.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the mapped output value.</returns>
    public Task<TOut> Invoke(TIn parameter, CancellationToken token)
    {
        return Map.InvokeAsync(parameter, token);
    }

    /// <summary>
    ///     Returns introspection metadata describing this mapping function.
    /// </summary>
    /// <returns>
    ///     An <see cref="IMappingFunctionInfo"/> describing the mapping function's descriptor, input type
    ///     parameters, and result types.
    /// </returns>
    public IMappingFunctionInfo GetInfo()
    {
        return new MappingFunctionInfo(Descriptor, [typeof(TIn)], [typeof(TOut)]);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TIn1, TIn2, TOut}"/> that represents an
///     asynchronous mapping function accepting two input parameters used by the state machine during a
///     parameter-mapped transition.
/// </summary>
/// <remarks>
///     This wrapper exists to give mapping functions a distinct nominal type rather than relying on bare delegate
///     types, which allows additional metadata to be associated with mappings in the future without changing call
///     sites.
/// </remarks>
/// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TOut">The type of the output parameter produced by the mapping function.</typeparam>
/// <param name="Map">The underlying asynchronous function that is invoked when the mapping runs.</param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncMap<TIn1, TIn2, TOut>(IAsyncFunc<TIn1, TIn2, TOut> Map, string? Descriptor)
{
    /// <summary>
    ///     Invokes the wrapped mapping function with the provided input parameters.
    /// </summary>
    /// <param name="parameter1">The first input parameter forwarded to the mapping function.</param>
    /// <param name="parameter2">The second input parameter forwarded to the mapping function.</param>
    /// <param name="token">The token to monitor for cancellation requests.</param>
    /// <returns>A task that completes with the mapped output value.</returns>
    public Task<TOut> Invoke(TIn1 parameter1, TIn2 parameter2, CancellationToken token)
    {
        return Map.InvokeAsync(parameter1, parameter2, token);
    }

    /// <summary>
    ///     Returns introspection metadata describing this mapping function.
    /// </summary>
    /// <returns>
    ///     An <see cref="IMappingFunctionInfo"/> describing the mapping function's descriptor, input type
    ///     parameters, and result types.
    /// </returns>
    public IMappingFunctionInfo GetInfo()
    {
        return new MappingFunctionInfo(Descriptor, [typeof(TIn1), typeof(TIn2)], [typeof(TOut)]);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TIn1, TIn2, TIn3, TOut}"/> that represents an
///     asynchronous mapping function accepting three input parameters used by the state machine during a
///     parameter-mapped transition.
/// </summary>
/// <remarks>
///     This wrapper exists to give mapping functions a distinct nominal type rather than relying on bare delegate
///     types, which allows additional metadata to be associated with mappings in the future without changing call
///     sites.
/// </remarks>
/// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
/// <typeparam name="TOut">The type of the output parameter produced by the mapping function.</typeparam>
/// <param name="Map">The underlying asynchronous function that is invoked when the mapping runs.</param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncMap<TIn1, TIn2, TIn3, TOut>(
    IAsyncFunc<TIn1, TIn2, TIn3, TOut> Map,
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
    /// <returns>A task that completes with the mapped output value.</returns>
    public Task<TOut> Invoke(TIn1 parameter1, TIn2 parameter2, TIn3 parameter3, CancellationToken token)
    {
        return Map.InvokeAsync(parameter1, parameter2, parameter3, token);
    }

    /// <summary>
    ///     Returns introspection metadata describing this mapping function.
    /// </summary>
    /// <returns>
    ///     An <see cref="IMappingFunctionInfo"/> describing the mapping function's descriptor, input type
    ///     parameters, and result types.
    /// </returns>
    public IMappingFunctionInfo GetInfo()
    {
        return new MappingFunctionInfo(Descriptor, [typeof(TIn1), typeof(TIn2), typeof(TIn3)], [typeof(TOut)]);
    }
}

/// <summary>
///     A strongly-typed wrapper around an <see cref="IAsyncFunc{TIn1, TIn2, TIn3, TIn4, TOut}"/> that represents
///     an asynchronous mapping function accepting four input parameters used by the state machine during a
///     parameter-mapped transition.
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
/// <typeparam name="TOut">The type of the output parameter produced by the mapping function.</typeparam>
/// <param name="Map">The underlying asynchronous function that is invoked when the mapping runs.</param>
/// <param name="Descriptor">An optional descriptor identifying the mapping, captured from the caller's expression.</param>
internal readonly record struct AsyncMap<TIn1, TIn2, TIn3, TIn4, TOut>(
    IAsyncFunc<TIn1, TIn2, TIn3, TIn4, TOut> Map,
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
    /// <returns>A task that completes with the mapped output value.</returns>
    public Task<TOut> Invoke(
        TIn1 parameter1,
        TIn2 parameter2,
        TIn3 parameter3,
        TIn4 parameter4,
        CancellationToken token
    )
    {
        return Map.InvokeAsync(parameter1, parameter2, parameter3, parameter4, token);
    }

    /// <summary>
    ///     Returns introspection metadata describing this mapping function.
    /// </summary>
    /// <returns>
    ///     An <see cref="IMappingFunctionInfo"/> describing the mapping function's descriptor, input type
    ///     parameters, and result types.
    /// </returns>
    public IMappingFunctionInfo GetInfo()
    {
        return new MappingFunctionInfo(
            Descriptor,
            [typeof(TIn1), typeof(TIn2), typeof(TIn3), typeof(TIn4)],
            [typeof(TOut)]
        );
    }
}
