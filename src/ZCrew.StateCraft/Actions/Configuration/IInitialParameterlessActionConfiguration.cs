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
public interface IInitialParameterlessActionConfiguration : IParameterlessActionConfiguration
{
    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IFinalParameterlessActionConfiguration Invoke(Action action);

    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IFinalParameterlessActionConfiguration Invoke(Func<CancellationToken, Task> action);

    /// <summary>
    ///     Configures the <paramref name="action"/> delegate to invoke when the action is executed.
    /// </summary>
    /// <param name="action">The delegate to invoke.</param>
    /// <returns>The final action configuration.</returns>
    IFinalParameterlessActionConfiguration Invoke(Func<CancellationToken, ValueTask> action);
}
