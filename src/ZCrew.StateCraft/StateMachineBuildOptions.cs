namespace ZCrew.StateCraft;

/// <summary>
///     Options to configure behavior when building a state machine.
/// </summary>
[Flags]
public enum StateMachineBuildOptions
{
    /// <summary>
    ///     No options. Does not perform validation.
    ///     Use <c>Build()</c> with no arguments or <c>Build(StateMachineBuildOptions.Validate)</c>
    ///     for build-time validation.
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
