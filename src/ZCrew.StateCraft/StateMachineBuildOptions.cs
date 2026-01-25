namespace ZCrew.StateCraft;

/// <summary>
///     Options to enable certain features when building a state machine.
/// </summary>
[Flags]
public enum StateMachineBuildOptions
{
    /// <summary>
    ///     Default option, which performs no additional build steps.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Performs validation on the state machine configuration before building an instance of it. This will ensure
    ///     most runtime exceptions are caught ahead-of-time, such as:
    ///     <list type="number">
    ///         <item>Duplicated states (including parameters)</item>
    ///         <item>Transitions to non-configured states (including parameters)</item>
    ///         <item>Unreachable transitions that can not be performed due to a previous transition</item>
    ///     </list>
    /// </summary>
    Validate = 1 << 0,
}
