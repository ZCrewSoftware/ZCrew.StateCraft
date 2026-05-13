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

    /// <summary>
    ///     Wraps a single-parameter asynchronous mapping function as an <see cref="AsyncMap{TIn, TOut}"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut">The type of the output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>An <see cref="AsyncMap{TIn, TOut}"/> that delegates invocation to <paramref name="func"/>.</returns>
    public static AsyncMap<TIn, TOut> AsAsyncMap<TIn, TOut>(this IAsyncFunc<TIn, TOut> func, string? descriptor = null)
    {
        return new AsyncMap<TIn, TOut>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a two-parameter asynchronous mapping function as an <see cref="AsyncMap{TIn1, TIn2, TOut}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut">The type of the output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMap{TIn1, TIn2, TOut}"/> that delegates invocation to <paramref name="func"/>.
    /// </returns>
    public static AsyncMap<TIn1, TIn2, TOut> AsAsyncMap<TIn1, TIn2, TOut>(
        this IAsyncFunc<TIn1, TIn2, TOut> func,
        string? descriptor = null
    )
    {
        return new AsyncMap<TIn1, TIn2, TOut>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a three-parameter asynchronous mapping function as an
    ///     <see cref="AsyncMap{TIn1, TIn2, TIn3, TOut}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut">The type of the output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMap{TIn1, TIn2, TIn3, TOut}"/> that delegates invocation to <paramref name="func"/>.
    /// </returns>
    public static AsyncMap<TIn1, TIn2, TIn3, TOut> AsAsyncMap<TIn1, TIn2, TIn3, TOut>(
        this IAsyncFunc<TIn1, TIn2, TIn3, TOut> func,
        string? descriptor = null
    )
    {
        return new AsyncMap<TIn1, TIn2, TIn3, TOut>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a four-parameter asynchronous mapping function as an
    ///     <see cref="AsyncMap{TIn1, TIn2, TIn3, TIn4, TOut}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn4">The type of the fourth input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut">The type of the output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMap{TIn1, TIn2, TIn3, TIn4, TOut}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMap<TIn1, TIn2, TIn3, TIn4, TOut> AsAsyncMap<TIn1, TIn2, TIn3, TIn4, TOut>(
        this IAsyncFunc<TIn1, TIn2, TIn3, TIn4, TOut> func,
        string? descriptor = null
    )
    {
        return new AsyncMap<TIn1, TIn2, TIn3, TIn4, TOut>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a single-parameter asynchronous mapping function producing a two-element value tuple as an
    ///     <see cref="AsyncMapValueTuple2{TIn, TOut1, TOut2}"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple2{TIn, TOut1, TOut2}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple2<TIn, TOut1, TOut2> AsAsyncMap<TIn, TOut1, TOut2>(
        this IAsyncFunc<TIn, (TOut1, TOut2)> func,
        string? descriptor = null
    )
    {
        return new AsyncMapValueTuple2<TIn, TOut1, TOut2>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a two-parameter asynchronous mapping function producing a two-element value tuple as an
    ///     <see cref="AsyncMapValueTuple2{TIn1, TIn2, TOut1, TOut2}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple2{TIn1, TIn2, TOut1, TOut2}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple2<TIn1, TIn2, TOut1, TOut2> AsAsyncMap<TIn1, TIn2, TOut1, TOut2>(
        this IAsyncFunc<TIn1, TIn2, (TOut1, TOut2)> func,
        string? descriptor = null
    )
    {
        return new AsyncMapValueTuple2<TIn1, TIn2, TOut1, TOut2>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a three-parameter asynchronous mapping function producing a two-element value tuple as an
    ///     <see cref="AsyncMapValueTuple2{TIn1, TIn2, TIn3, TOut1, TOut2}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple2{TIn1, TIn2, TIn3, TOut1, TOut2}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple2<TIn1, TIn2, TIn3, TOut1, TOut2> AsAsyncMap<TIn1, TIn2, TIn3, TOut1, TOut2>(
        this IAsyncFunc<TIn1, TIn2, TIn3, (TOut1, TOut2)> func,
        string? descriptor = null
    )
    {
        return new AsyncMapValueTuple2<TIn1, TIn2, TIn3, TOut1, TOut2>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a four-parameter asynchronous mapping function producing a two-element value tuple as an
    ///     <see cref="AsyncMapValueTuple2{TIn1, TIn2, TIn3, TIn4, TOut1, TOut2}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn4">The type of the fourth input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple2{TIn1, TIn2, TIn3, TIn4, TOut1, TOut2}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple2<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2> AsAsyncMap<
        TIn1,
        TIn2,
        TIn3,
        TIn4,
        TOut1,
        TOut2
    >(this IAsyncFunc<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2)> func, string? descriptor = null)
    {
        return new AsyncMapValueTuple2<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a single-parameter asynchronous mapping function producing a three-element value tuple as an
    ///     <see cref="AsyncMapValueTuple3{TIn, TOut1, TOut2, TOut3}"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut3">The type of the third output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple3{TIn, TOut1, TOut2, TOut3}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple3<TIn, TOut1, TOut2, TOut3> AsAsyncMap<TIn, TOut1, TOut2, TOut3>(
        this IAsyncFunc<TIn, (TOut1, TOut2, TOut3)> func,
        string? descriptor = null
    )
    {
        return new AsyncMapValueTuple3<TIn, TOut1, TOut2, TOut3>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a two-parameter asynchronous mapping function producing a three-element value tuple as an
    ///     <see cref="AsyncMapValueTuple3{TIn1, TIn2, TOut1, TOut2, TOut3}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut3">The type of the third output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple3{TIn1, TIn2, TOut1, TOut2, TOut3}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple3<TIn1, TIn2, TOut1, TOut2, TOut3> AsAsyncMap<TIn1, TIn2, TOut1, TOut2, TOut3>(
        this IAsyncFunc<TIn1, TIn2, (TOut1, TOut2, TOut3)> func,
        string? descriptor = null
    )
    {
        return new AsyncMapValueTuple3<TIn1, TIn2, TOut1, TOut2, TOut3>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a three-parameter asynchronous mapping function producing a three-element value tuple as an
    ///     <see cref="AsyncMapValueTuple3{TIn1, TIn2, TIn3, TOut1, TOut2, TOut3}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut3">The type of the third output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple3{TIn1, TIn2, TIn3, TOut1, TOut2, TOut3}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple3<TIn1, TIn2, TIn3, TOut1, TOut2, TOut3> AsAsyncMap<
        TIn1,
        TIn2,
        TIn3,
        TOut1,
        TOut2,
        TOut3
    >(this IAsyncFunc<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3)> func, string? descriptor = null)
    {
        return new AsyncMapValueTuple3<TIn1, TIn2, TIn3, TOut1, TOut2, TOut3>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a four-parameter asynchronous mapping function producing a three-element value tuple as an
    ///     <see cref="AsyncMapValueTuple3{TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn4">The type of the fourth input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut3">The type of the third output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple3{TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3}"/> that delegates
    ///     invocation to <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple3<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3> AsAsyncMap<
        TIn1,
        TIn2,
        TIn3,
        TIn4,
        TOut1,
        TOut2,
        TOut3
    >(this IAsyncFunc<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3)> func, string? descriptor = null)
    {
        return new AsyncMapValueTuple3<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a single-parameter asynchronous mapping function producing a four-element value tuple as an
    ///     <see cref="AsyncMapValueTuple4{TIn, TOut1, TOut2, TOut3, TOut4}"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut3">The type of the third output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut4">The type of the fourth output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple4{TIn, TOut1, TOut2, TOut3, TOut4}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple4<TIn, TOut1, TOut2, TOut3, TOut4> AsAsyncMap<TIn, TOut1, TOut2, TOut3, TOut4>(
        this IAsyncFunc<TIn, (TOut1, TOut2, TOut3, TOut4)> func,
        string? descriptor = null
    )
    {
        return new AsyncMapValueTuple4<TIn, TOut1, TOut2, TOut3, TOut4>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a two-parameter asynchronous mapping function producing a four-element value tuple as an
    ///     <see cref="AsyncMapValueTuple4{TIn1, TIn2, TOut1, TOut2, TOut3, TOut4}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut3">The type of the third output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut4">The type of the fourth output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple4{TIn1, TIn2, TOut1, TOut2, TOut3, TOut4}"/> that delegates invocation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple4<TIn1, TIn2, TOut1, TOut2, TOut3, TOut4> AsAsyncMap<
        TIn1,
        TIn2,
        TOut1,
        TOut2,
        TOut3,
        TOut4
    >(this IAsyncFunc<TIn1, TIn2, (TOut1, TOut2, TOut3, TOut4)> func, string? descriptor = null)
    {
        return new AsyncMapValueTuple4<TIn1, TIn2, TOut1, TOut2, TOut3, TOut4>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a three-parameter asynchronous mapping function producing a four-element value tuple as an
    ///     <see cref="AsyncMapValueTuple4{TIn1, TIn2, TIn3, TOut1, TOut2, TOut3, TOut4}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut3">The type of the third output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut4">The type of the fourth output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple4{TIn1, TIn2, TIn3, TOut1, TOut2, TOut3, TOut4}"/> that delegates
    ///     invocation to <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple4<TIn1, TIn2, TIn3, TOut1, TOut2, TOut3, TOut4> AsAsyncMap<
        TIn1,
        TIn2,
        TIn3,
        TOut1,
        TOut2,
        TOut3,
        TOut4
    >(this IAsyncFunc<TIn1, TIn2, TIn3, (TOut1, TOut2, TOut3, TOut4)> func, string? descriptor = null)
    {
        return new AsyncMapValueTuple4<TIn1, TIn2, TIn3, TOut1, TOut2, TOut3, TOut4>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a four-parameter asynchronous mapping function producing a four-element value tuple as an
    ///     <see cref="AsyncMapValueTuple4{TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3, TOut4}"/>.
    /// </summary>
    /// <typeparam name="TIn1">The type of the first input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn2">The type of the second input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn3">The type of the third input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TIn4">The type of the fourth input parameter consumed by the mapping function.</typeparam>
    /// <typeparam name="TOut1">The type of the first output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut2">The type of the second output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut3">The type of the third output parameter produced by the mapping function.</typeparam>
    /// <typeparam name="TOut4">The type of the fourth output parameter produced by the mapping function.</typeparam>
    /// <param name="func">The asynchronous mapping function to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the mapping. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncMapValueTuple4{TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3, TOut4}"/> that delegates
    ///     invocation to <paramref name="func"/>.
    /// </returns>
    public static AsyncMapValueTuple4<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3, TOut4> AsAsyncMap<
        TIn1,
        TIn2,
        TIn3,
        TIn4,
        TOut1,
        TOut2,
        TOut3,
        TOut4
    >(this IAsyncFunc<TIn1, TIn2, TIn3, TIn4, (TOut1, TOut2, TOut3, TOut4)> func, string? descriptor = null)
    {
        return new AsyncMapValueTuple4<TIn1, TIn2, TIn3, TIn4, TOut1, TOut2, TOut3, TOut4>(func, descriptor);
    }

    /// <summary>
    ///     Wraps a parameterless asynchronous initial state provider as an
    ///     <see cref="AsyncStateProvider{TState}"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the initial state value.</typeparam>
    /// <param name="func">The asynchronous state provider to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncStateProvider{TState}"/> that delegates evaluation to <paramref name="func"/>.
    /// </returns>
    public static AsyncStateProvider<TState> AsAsyncStateProvider<TState>(
        this IAsyncFunc<TState> func,
        string? descriptor = null
    )
        where TState : notnull
    {
        return new AsyncStateProvider<TState>(func, descriptor);
    }

    /// <summary>
    ///     Wraps an asynchronous initial state provider returning a state and one parameter as an
    ///     <see cref="AsyncStateProvider{TState, T}"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the initial state value.</typeparam>
    /// <typeparam name="T">The type of the initial parameter.</typeparam>
    /// <param name="func">The asynchronous state provider to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncStateProvider{TState, T}"/> that delegates evaluation to <paramref name="func"/>.
    /// </returns>
    public static AsyncStateProvider<TState, T> AsAsyncStateProvider<TState, T>(
        this IAsyncFunc<(TState, T)> func,
        string? descriptor = null
    )
        where TState : notnull
    {
        return new AsyncStateProvider<TState, T>(func, descriptor);
    }

    /// <summary>
    ///     Wraps an asynchronous initial state provider returning a state and two parameters as an
    ///     <see cref="AsyncStateProvider{TState, T1, T2}"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the initial state value.</typeparam>
    /// <typeparam name="T1">The type of the first initial parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial parameter.</typeparam>
    /// <param name="func">The asynchronous state provider to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncStateProvider{TState, T1, T2}"/> that delegates evaluation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncStateProvider<TState, T1, T2> AsAsyncStateProvider<TState, T1, T2>(
        this IAsyncFunc<(TState, T1, T2)> func,
        string? descriptor = null
    )
        where TState : notnull
    {
        return new AsyncStateProvider<TState, T1, T2>(func, descriptor);
    }

    /// <summary>
    ///     Wraps an asynchronous initial state provider returning a state and three parameters as an
    ///     <see cref="AsyncStateProvider{TState, T1, T2, T3}"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the initial state value.</typeparam>
    /// <typeparam name="T1">The type of the first initial parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial parameter.</typeparam>
    /// <param name="func">The asynchronous state provider to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncStateProvider{TState, T1, T2, T3}"/> that delegates evaluation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncStateProvider<TState, T1, T2, T3> AsAsyncStateProvider<TState, T1, T2, T3>(
        this IAsyncFunc<(TState, T1, T2, T3)> func,
        string? descriptor = null
    )
        where TState : notnull
    {
        return new AsyncStateProvider<TState, T1, T2, T3>(func, descriptor);
    }

    /// <summary>
    ///     Wraps an asynchronous initial state provider returning a state and four parameters as an
    ///     <see cref="AsyncStateProvider{TState, T1, T2, T3, T4}"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the initial state value.</typeparam>
    /// <typeparam name="T1">The type of the first initial parameter.</typeparam>
    /// <typeparam name="T2">The type of the second initial parameter.</typeparam>
    /// <typeparam name="T3">The type of the third initial parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth initial parameter.</typeparam>
    /// <param name="func">The asynchronous state provider to wrap.</param>
    /// <param name="descriptor">
    ///     An optional descriptor identifying the state provider. When omitted, no descriptor is captured.
    /// </param>
    /// <returns>
    ///     An <see cref="AsyncStateProvider{TState, T1, T2, T3, T4}"/> that delegates evaluation to
    ///     <paramref name="func"/>.
    /// </returns>
    public static AsyncStateProvider<TState, T1, T2, T3, T4> AsAsyncStateProvider<TState, T1, T2, T3, T4>(
        this IAsyncFunc<(TState, T1, T2, T3, T4)> func,
        string? descriptor = null
    )
        where TState : notnull
    {
        return new AsyncStateProvider<TState, T1, T2, T3, T4>(func, descriptor);
    }
}
