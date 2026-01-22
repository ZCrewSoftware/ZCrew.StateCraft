namespace ZCrew.StateCraft;

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
public interface IInitialParameterizedActionConfiguration<T> : IParameterizedActionConfiguration<T>
{
    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IFinalParameterizedActionConfiguration<T> Invoke(Action<T> action);

    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IFinalParameterizedActionConfiguration<T> Invoke(Func<T, CancellationToken, Task> action);

    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IFinalParameterizedActionConfiguration<T> Invoke(Func<T, CancellationToken, ValueTask> action);
}
