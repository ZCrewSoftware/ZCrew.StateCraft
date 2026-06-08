using ZCrew.StateCraft.Async;
using ZCrew.StateCraft.Async.Contracts;

namespace ZCrew.StateCraft.Extensions;

/// <summary>
///     Extension methods for converting <see cref="AsyncHandler"/> wrappers and their parameterized counterparts into
///     <see cref="INextParametersHandler"/> instances that pull their arguments from the next-state parameters.
/// </summary>
/// <remarks>
///     <see cref="ParametersHandler"/> implements all three handler directions
///     (<see cref="INextParametersHandler"/>, <c>ICurrentParametersHandler</c>, and <c>IPreviousParametersHandler</c>),
///     so a single conversion would be ambiguous about which direction is intended. These methods make the next-state
///     direction explicit; sibling <c>AsPreviousParametersHandler</c> / <c>AsCurrentParametersHandler</c> conversions
///     can be added with the same shape when a handler that reads those directions is introduced.
/// </remarks>
internal static class ParametersHandlerExtensions
{
    /// <summary>
    ///     Wraps a parameterless <see cref="AsyncHandler"/> as an <see cref="INextParametersHandler"/>.
    /// </summary>
    /// <param name="handler">The handler to wrap.</param>
    /// <returns>An <see cref="INextParametersHandler"/> that invokes <paramref name="handler"/>.</returns>
    public static INextParametersHandler AsNextParametersHandler(this AsyncHandler handler)
    {
        return new ParametersHandler(handler);
    }

    /// <summary>
    ///     Wraps a single-parameter <see cref="AsyncHandler{T}"/> as an <see cref="INextParametersHandler"/> that
    ///     supplies the next-state parameter when invoked.
    /// </summary>
    /// <typeparam name="T">The type of the next-state parameter passed to the handler.</typeparam>
    /// <param name="handler">The handler to wrap.</param>
    /// <returns>An <see cref="INextParametersHandler"/> that invokes <paramref name="handler"/>.</returns>
    public static INextParametersHandler AsNextParametersHandler<T>(this AsyncHandler<T> handler)
    {
        return new ParametersHandler<T>(handler);
    }

    /// <summary>
    ///     Wraps a two-parameter <see cref="AsyncHandler{T1, T2}"/> as an <see cref="INextParametersHandler"/> that
    ///     supplies the next-state parameters when invoked.
    /// </summary>
    /// <typeparam name="T1">The type of the first next-state parameter passed to the handler.</typeparam>
    /// <typeparam name="T2">The type of the second next-state parameter passed to the handler.</typeparam>
    /// <param name="handler">The handler to wrap.</param>
    /// <returns>An <see cref="INextParametersHandler"/> that invokes <paramref name="handler"/>.</returns>
    public static INextParametersHandler AsNextParametersHandler<T1, T2>(this AsyncHandler<T1, T2> handler)
    {
        return new ParametersHandler<T1, T2>(handler);
    }

    /// <summary>
    ///     Wraps a three-parameter <see cref="AsyncHandler{T1, T2, T3}"/> as an <see cref="INextParametersHandler"/>
    ///     that supplies the next-state parameters when invoked.
    /// </summary>
    /// <typeparam name="T1">The type of the first next-state parameter passed to the handler.</typeparam>
    /// <typeparam name="T2">The type of the second next-state parameter passed to the handler.</typeparam>
    /// <typeparam name="T3">The type of the third next-state parameter passed to the handler.</typeparam>
    /// <param name="handler">The handler to wrap.</param>
    /// <returns>An <see cref="INextParametersHandler"/> that invokes <paramref name="handler"/>.</returns>
    public static INextParametersHandler AsNextParametersHandler<T1, T2, T3>(this AsyncHandler<T1, T2, T3> handler)
    {
        return new ParametersHandler<T1, T2, T3>(handler);
    }

    /// <summary>
    ///     Wraps a four-parameter <see cref="AsyncHandler{T1, T2, T3, T4}"/> as an
    ///     <see cref="INextParametersHandler"/> that supplies the next-state parameters when invoked.
    /// </summary>
    /// <typeparam name="T1">The type of the first next-state parameter passed to the handler.</typeparam>
    /// <typeparam name="T2">The type of the second next-state parameter passed to the handler.</typeparam>
    /// <typeparam name="T3">The type of the third next-state parameter passed to the handler.</typeparam>
    /// <typeparam name="T4">The type of the fourth next-state parameter passed to the handler.</typeparam>
    /// <param name="handler">The handler to wrap.</param>
    /// <returns>An <see cref="INextParametersHandler"/> that invokes <paramref name="handler"/>.</returns>
    public static INextParametersHandler AsNextParametersHandler<T1, T2, T3, T4>(
        this AsyncHandler<T1, T2, T3, T4> handler
    )
    {
        return new ParametersHandler<T1, T2, T3, T4>(handler);
    }
}
