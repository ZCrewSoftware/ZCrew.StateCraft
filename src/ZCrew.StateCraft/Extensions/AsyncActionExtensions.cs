using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Async;

namespace ZCrew.StateCraft.Extensions;

/// <summary>
///     Extension methods for converting <see cref="IAsyncAction"/> instances and their parameterized counterparts
///     into the strongly-typed <see cref="AsyncHandler"/> wrapper types.
/// </summary>
/// <remarks>
///     These conversions exist so that the state machine can work with a distinct handler type rather than a bare
///     action delegate, allowing future metadata to be attached to handlers without changing call sites.
/// </remarks>
internal static class AsyncActionExtensions
{
    /// <summary>
    ///     Wraps a parameterless asynchronous action as an <see cref="AsyncHandler"/>.
    /// </summary>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncHandler"/> that delegates invocation to <paramref name="action"/>.</returns>
    public static AsyncHandler AsAsyncHandler(this IAsyncAction action, string? descriptor = null)
    {
        return new AsyncHandler(action, descriptor);
    }

    /// <summary>
    ///     Wraps a single-parameter asynchronous action as an <see cref="AsyncHandler{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter passed to the action.</typeparam>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncHandler{T}"/> that delegates invocation to <paramref name="action"/>.</returns>
    public static AsyncHandler<T> AsAsyncHandler<T>(this IAsyncAction<T> action, string? descriptor = null)
    {
        return new AsyncHandler<T>(action, descriptor);
    }

    /// <summary>
    ///     Wraps a two-parameter asynchronous action as an <see cref="AsyncHandler{T1, T2}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncHandler{T1, T2}"/> that delegates invocation to <paramref name="action"/>.</returns>
    public static AsyncHandler<T1, T2> AsAsyncHandler<T1, T2>(
        this IAsyncAction<T1, T2> action,
        string? descriptor = null
    )
    {
        return new AsyncHandler<T1, T2>(action, descriptor);
    }

    /// <summary>
    ///     Wraps a three-parameter asynchronous action as an <see cref="AsyncHandler{T1, T2, T3}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
    /// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncHandler{T1, T2, T3}"/> that delegates invocation to <paramref name="action"/>.</returns>
    public static AsyncHandler<T1, T2, T3> AsAsyncHandler<T1, T2, T3>(
        this IAsyncAction<T1, T2, T3> action,
        string? descriptor = null
    )
    {
        return new AsyncHandler<T1, T2, T3>(action, descriptor);
    }

    /// <summary>
    ///     Wraps a four-parameter asynchronous action as an <see cref="AsyncHandler{T1, T2, T3, T4}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
    /// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter passed to the action.</typeparam>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncHandler{T1, T2, T3, T4}"/> that delegates invocation to <paramref name="action"/>.
    /// </returns>
    public static AsyncHandler<T1, T2, T3, T4> AsAsyncHandler<T1, T2, T3, T4>(
        this IAsyncAction<T1, T2, T3, T4> action,
        string? descriptor = null
    )
    {
        return new AsyncHandler<T1, T2, T3, T4>(action, descriptor);
    }

    /// <summary>
    ///     Wraps a five-parameter asynchronous action as an <see cref="AsyncHandler{T1, T2, T3, T4, T5}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
    /// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter passed to the action.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter passed to the action.</typeparam>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncHandler{T1, T2, T3, T4, T5}"/> that delegates invocation to <paramref name="action"/>.
    /// </returns>
    public static AsyncHandler<T1, T2, T3, T4, T5> AsAsyncHandler<T1, T2, T3, T4, T5>(
        this IAsyncAction<T1, T2, T3, T4, T5> action,
        string? descriptor = null
    )
    {
        return new AsyncHandler<T1, T2, T3, T4, T5>(action, descriptor);
    }

    /// <summary>
    ///     Wraps a six-parameter asynchronous action as an <see cref="AsyncHandler{T1, T2, T3, T4, T5, T6}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
    /// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter passed to the action.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter passed to the action.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter passed to the action.</typeparam>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncHandler{T1, T2, T3, T4, T5, T6}"/> that delegates invocation to <paramref name="action"/>.
    /// </returns>
    public static AsyncHandler<T1, T2, T3, T4, T5, T6> AsAsyncHandler<T1, T2, T3, T4, T5, T6>(
        this IAsyncAction<T1, T2, T3, T4, T5, T6> action,
        string? descriptor = null
    )
    {
        return new AsyncHandler<T1, T2, T3, T4, T5, T6>(action, descriptor);
    }

    /// <summary>
    ///     Wraps a seven-parameter asynchronous action as an <see cref="AsyncHandler{T1, T2, T3, T4, T5, T6, T7}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
    /// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter passed to the action.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter passed to the action.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter passed to the action.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter passed to the action.</typeparam>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncHandler{T1, T2, T3, T4, T5, T6, T7}"/> that delegates invocation to
    ///     <paramref name="action"/>.
    /// </returns>
    public static AsyncHandler<T1, T2, T3, T4, T5, T6, T7> AsAsyncHandler<T1, T2, T3, T4, T5, T6, T7>(
        this IAsyncAction<T1, T2, T3, T4, T5, T6, T7> action,
        string? descriptor = null
    )
    {
        return new AsyncHandler<T1, T2, T3, T4, T5, T6, T7>(action, descriptor);
    }

    /// <summary>
    ///     Wraps an eight-parameter asynchronous action as an
    ///     <see cref="AsyncHandler{T1, T2, T3, T4, T5, T6, T7, T8}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
    /// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter passed to the action.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter passed to the action.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter passed to the action.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter passed to the action.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter passed to the action.</typeparam>
    /// <param name="action">The asynchronous action to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the handler. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncHandler{T1, T2, T3, T4, T5, T6, T7, T8}"/> that delegates invocation to
    ///     <paramref name="action"/>.
    /// </returns>
    public static AsyncHandler<T1, T2, T3, T4, T5, T6, T7, T8> AsAsyncHandler<T1, T2, T3, T4, T5, T6, T7, T8>(
        this IAsyncAction<T1, T2, T3, T4, T5, T6, T7, T8> action,
        string? descriptor = null
    )
    {
        return new AsyncHandler<T1, T2, T3, T4, T5, T6, T7, T8>(action, descriptor);
    }
}
