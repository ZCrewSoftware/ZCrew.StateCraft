using ZCrew.Extensions.Tasks;
using ZCrew.StateCraft.Async;

namespace ZCrew.StateCraft.Extensions;

/// <summary>
///     Extension methods for converting <see cref="IAsyncFunc{Boolean}"/> instances and their parameterized
///     counterparts into the strongly-typed <see cref="AsyncCondition"/> wrapper types.
/// </summary>
/// <remarks>
///     These conversions exist so that the state machine can work with a distinct condition type rather than a bare
///     boolean-returning delegate, allowing future metadata to be attached to conditions without changing call sites.
/// </remarks>
internal static class AsyncFuncExtensions
{
    /// <summary>
    ///     Wraps a parameterless asynchronous predicate as an <see cref="AsyncCondition"/>.
    /// </summary>
    /// <param name="func">The asynchronous predicate to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the condition. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncCondition"/> that delegates evaluation to <paramref name="func"/>.</returns>
    public static AsyncCondition AsAsyncCondition(this IAsyncFunc<bool> func, string? descriptor = null)
    {
        return new AsyncCondition(func, descriptor);
    }

    /// <summary>
    ///     Wraps a single-parameter asynchronous predicate as an <see cref="AsyncCondition{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter passed to the predicate.</typeparam>
    /// <param name="func">The asynchronous predicate to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the condition. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncCondition{T}"/> that delegates evaluation to <paramref name="func"/>.</returns>
    public static AsyncCondition<T> AsAsyncCondition<T>(this IAsyncFunc<T, bool> func, string? descriptor = null)
    {
        return new AsyncCondition<T>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a two-parameter asynchronous predicate as an <see cref="AsyncCondition{T1, T2}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the predicate.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the predicate.</typeparam>
    /// <param name="func">The asynchronous predicate to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the condition. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncCondition{T1, T2}"/> that delegates evaluation to <paramref name="func"/>.</returns>
    public static AsyncCondition<T1, T2> AsAsyncCondition<T1, T2>(
        this IAsyncFunc<T1, T2, bool> func,
        string? descriptor = null
    )
    {
        return new AsyncCondition<T1, T2>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a three-parameter asynchronous predicate as an <see cref="AsyncCondition{T1, T2, T3}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the predicate.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the predicate.</typeparam>
    /// <typeparam name="T3">The type of the third parameter passed to the predicate.</typeparam>
    /// <param name="func">The asynchronous predicate to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the condition. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncCondition{T1, T2, T3}"/> that delegates evaluation to <paramref name="func"/>.</returns>
    public static AsyncCondition<T1, T2, T3> AsAsyncCondition<T1, T2, T3>(
        this IAsyncFunc<T1, T2, T3, bool> func,
        string? descriptor = null
    )
    {
        return new AsyncCondition<T1, T2, T3>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a four-parameter asynchronous predicate as an <see cref="AsyncCondition{T1, T2, T3, T4}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter passed to the predicate.</typeparam>
    /// <typeparam name="T2">The type of the second parameter passed to the predicate.</typeparam>
    /// <typeparam name="T3">The type of the third parameter passed to the predicate.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter passed to the predicate.</typeparam>
    /// <param name="func">The asynchronous predicate to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the condition. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncCondition{T1, T2, T3, T4}"/> that delegates evaluation to <paramref name="func"/>.
    /// </returns>
    public static AsyncCondition<T1, T2, T3, T4> AsAsyncCondition<T1, T2, T3, T4>(
        this IAsyncFunc<T1, T2, T3, T4, bool> func,
        string? descriptor = null
    )
    {
        return new AsyncCondition<T1, T2, T3, T4>(func, descriptor);
    }
}
