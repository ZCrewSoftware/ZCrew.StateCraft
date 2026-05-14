namespace ZCrew.StateCraft;

/// <summary>
///     Common shape for introspection metadata describing a delegate captured during configuration. Identifies the
///     delegate by a human-readable descriptor and the type parameters of its signature.
/// </summary>
public interface IDelegateInfo
{
    /// <summary>
    ///     A human-readable label for the delegate, typically the caller's expression captured at configuration
    ///     time via <see cref="System.Runtime.CompilerServices.CallerArgumentExpressionAttribute"/>.
    ///     <see langword="null"/> when no descriptor was supplied.
    /// </summary>
    string? Descriptor { get; }

    /// <summary>
    ///     The generic type arguments of the delegate's signature, in declaration order. Empty for parameterless
    ///     delegates.
    /// </summary>
    IReadOnlyList<Type> TypeParameters { get; }
}
