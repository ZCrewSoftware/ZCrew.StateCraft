namespace ZCrew.StateCraft;

/// <summary>
///     Represents the result of exception handling, determining how the exception should be processed.
/// </summary>
/// <remarks>
///     If you are unsure of which value to use, it is safe to use <see cref="Continue"/> as this will invoke every
///     exception handler and then rethrow the exception.
/// </remarks>
public abstract record ExceptionResult
{
    private ExceptionResult() { }

    /// <summary>
    ///     Passes the exception to the next handler in the chain.
    ///     If no more handlers exist, the exception is rethrown.
    /// </summary>
    /// <returns>An <see cref="ExceptionResult"/> that indicates the next handler should be invoked.</returns>
    /// <remarks>
    ///     This typically a safe value to use if you are unsure.
    /// </remarks>
    public static ExceptionResult Continue()
    {
        return ContinueResult.instance;
    }

    /// <summary>
    ///     Rethrows the original exception, preserving the stack trace.
    /// </summary>
    /// <returns>An <see cref="ExceptionResult"/> that indicates the exception should be rethrown.</returns>
    public static ExceptionResult Rethrow()
    {
        return RethrowResult.instance;
    }

    /// <summary>
    ///     Throws a different exception instead of the original.
    /// </summary>
    /// <param name="exception">The exception to throw.</param>
    /// <returns>An <see cref="ExceptionResult"/> that indicates a different exception should be thrown.</returns>
    public static ExceptionResult Throw(Exception exception)
    {
        return new ThrowResult(exception);
    }

    /// <inheritdoc cref="ExceptionResult.Rethrow"/>
    public sealed record RethrowResult : ExceptionResult
    {
        internal static readonly RethrowResult instance = new();

        private RethrowResult() { }
    }

    /// <inheritdoc cref="ExceptionResult.Throw"/>
    public sealed record ThrowResult(Exception Exception) : ExceptionResult;

    /// <inheritdoc cref="ExceptionResult.Continue"/>
    public sealed record ContinueResult : ExceptionResult
    {
        internal static readonly ContinueResult instance = new();

        private ContinueResult() { }
    }
}
