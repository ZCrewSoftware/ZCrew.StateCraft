namespace ZCrew.StateCraft.Parameters.Contracts;

/// <summary>
///     A read-only view of one set of state parameters.
/// </summary>
internal interface IParameters
{
    /// <summary>
    ///     Gets a value indicating whether the parameters are set. A parameterless state is set with a
    ///     <see cref="Count"/> of zero.
    /// </summary>
    bool IsSet { get; }

    /// <summary>
    ///     The number of parameters. Between zero and four.
    /// </summary>
    int Count { get; }

    /// <summary>
    ///     The declared type of each parameter, in declaration order.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     If <see cref="IsSet"/> is not <see langword="true"/>.
    /// </exception>
    IReadOnlyList<Type> Types { get; }

    /// <summary>
    ///     The parameter values, in declaration order.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     If <see cref="IsSet"/> is not <see langword="true"/>.
    /// </exception>
    IReadOnlyList<object?> Values { get; }

    /// <summary>
    ///     Retrieves the single parameter.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If <see cref="IsSet"/> is not <see langword="true"/>, or if <see cref="Count"/> is not 1.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If the parameter cannot be cast to <typeparamref name="T"/>.
    /// </exception>
    T Get<T>();

    /// <summary>
    ///     Retrieves the two parameters.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If <see cref="IsSet"/> is not <see langword="true"/>, or if <see cref="Count"/> is not 2.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2) Get<T1, T2>();

    /// <summary>
    ///     Retrieves the three parameters.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <typeparam name="T3">The expected type of the third parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If <see cref="IsSet"/> is not <see langword="true"/>, or if <see cref="Count"/> is not 3.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2, T3) Get<T1, T2, T3>();

    /// <summary>
    ///     Retrieves the four parameters.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first parameter.</typeparam>
    /// <typeparam name="T2">The expected type of the second parameter.</typeparam>
    /// <typeparam name="T3">The expected type of the third parameter.</typeparam>
    /// <typeparam name="T4">The expected type of the fourth parameter.</typeparam>
    /// <returns>A tuple of the parameter values.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If <see cref="IsSet"/> is not <see langword="true"/>, or if <see cref="Count"/> is not 4.
    /// </exception>
    /// <exception cref="InvalidCastException">
    ///     If any parameter cannot be cast to its expected type.
    /// </exception>
    (T1, T2, T3, T4) Get<T1, T2, T3, T4>();
}
