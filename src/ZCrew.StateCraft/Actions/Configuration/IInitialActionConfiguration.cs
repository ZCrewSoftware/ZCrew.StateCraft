namespace ZCrew.StateCraft;

/// <summary>
///     The initial parameterless action configuration which provides an opportunity to specify the action delegate.
/// </summary>
/// <example>
///     <code>
///     .WithState("Running", state => state
///         .WithAction(action => action.Invoke(() => Console.WriteLine("Running"))))
///     </code>
/// </example>
public interface IInitialActionConfiguration
{
    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IActionConfiguration Invoke(Action action);

    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IActionConfiguration Invoke(Func<CancellationToken, Task> action);

    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IActionConfiguration Invoke(Func<CancellationToken, ValueTask> action);
}

/// <summary>
///     The initial parameterized action configuration which provides an opportunity to specify the action delegate.
/// </summary>
/// <typeparam name="T">The type of the parameter passed to the action.</typeparam>
/// <example>
///     <code>
///     .WithState("Processing", state => state
///         .WithParameter&lt;int&gt;()
///         .WithAction(action => action.Invoke(count => Console.WriteLine($"Processing {count} items"))))
///     </code>
/// </example>
public interface IInitialActionConfiguration<T>
{
    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Action)"/>
    IActionConfiguration<T> Invoke(Action<T> action);

    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Func{CancellationToken,Task})"/>
    IActionConfiguration<T> Invoke(Func<T, CancellationToken, Task> action);

    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Func{CancellationToken,ValueTask})"/>
    IActionConfiguration<T> Invoke(Func<T, CancellationToken, ValueTask> action);
}

/// <inheritdoc cref="IInitialActionConfiguration{T}"/>
/// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
public interface IInitialActionConfiguration<T1, T2>
{
    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Action)"/>
    IActionConfiguration<T1, T2> Invoke(Action<T1, T2> action);

    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Func{CancellationToken,Task})"/>
    IActionConfiguration<T1, T2> Invoke(Func<T1, T2, CancellationToken, Task> action);

    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Func{CancellationToken,ValueTask})"/>
    IActionConfiguration<T1, T2> Invoke(Func<T1, T2, CancellationToken, ValueTask> action);
}

/// <inheritdoc cref="IInitialActionConfiguration{T}"/>
/// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
public interface IInitialActionConfiguration<T1, T2, T3>
{
    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Action)"/>
    IActionConfiguration<T1, T2, T3> Invoke(Action<T1, T2, T3> action);

    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Func{CancellationToken,Task})"/>
    IActionConfiguration<T1, T2, T3> Invoke(Func<T1, T2, T3, CancellationToken, Task> action);

    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Func{CancellationToken,ValueTask})"/>
    IActionConfiguration<T1, T2, T3> Invoke(Func<T1, T2, T3, CancellationToken, ValueTask> action);
}

/// <inheritdoc cref="IInitialActionConfiguration{T}"/>
/// <typeparam name="T1">The type of the first parameter passed to the action.</typeparam>
/// <typeparam name="T2">The type of the second parameter passed to the action.</typeparam>
/// <typeparam name="T3">The type of the third parameter passed to the action.</typeparam>
/// <typeparam name="T4">The type of the fourth parameter passed to the action.</typeparam>
public interface IInitialActionConfiguration<T1, T2, T3, T4>
{
    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Action)"/>
    IActionConfiguration<T1, T2, T3, T4> Invoke(Action<T1, T2, T3, T4> action);

    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Func{CancellationToken,Task})"/>
    IActionConfiguration<T1, T2, T3, T4> Invoke(Func<T1, T2, T3, T4, CancellationToken, Task> action);

    /// <inheritdoc cref="IInitialActionConfiguration.Invoke(Func{CancellationToken,ValueTask})"/>
    IActionConfiguration<T1, T2, T3, T4> Invoke(Func<T1, T2, T3, T4, CancellationToken, ValueTask> action);
}
