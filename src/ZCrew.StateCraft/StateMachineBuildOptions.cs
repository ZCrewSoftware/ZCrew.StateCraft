namespace ZCrew.StateCraft;

/// <summary>
///     Options to configure behavior when building a state machine.
/// </summary>
[Flags]
public enum StateMachineBuildOptions
{
    /// <summary>
    ///     Default option. Performs validation on the state machine configuration before building an instance of it.
    ///     This will ensure most runtime exceptions are caught ahead-of-time, such as:
    ///     <list type="number">
    ///         <item>Duplicated states (including parameters)</item>
    ///         <item>Transitions to non-configured states (including parameters)</item>
    ///         <item>Unreachable transitions that can not be performed due to a previous transition</item>
    ///     </list>
    /// </summary>
    None = 0,

    /// <summary>
    ///     Performs validation on the state machine configuration before building an instance of it.
    ///     This is the default behavior when calling <c>Build()</c> with no arguments.
    /// </summary>
    Validate = 1 << 0,

    /// <summary>
    ///     Skips validation of the state machine configuration. Use this when building intentionally
    ///     incomplete or invalid configurations (e.g., in test harnesses).
    /// </summary>
    SkipValidation = 1 << 1,
}
