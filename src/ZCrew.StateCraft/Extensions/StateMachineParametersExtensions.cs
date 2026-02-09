using ZCrew.StateCraft.Parameters.Contracts;

namespace ZCrew.StateCraft.Extensions;

/// <summary>
///     Provides convenience properties for checking which parameter slots are set
///     on an <see cref="IStateMachineParameters"/> instance.
/// </summary>
internal static class StateMachineParametersExtensions
{
    extension(IStateMachineParameters parameters)
    {
        /// <summary>
        ///     Gets a value indicating whether the previous parameters slot is set.
        /// </summary>
        public bool IsPreviousSet => parameters.Status.HasFlag(StateMachineParametersFlags.PreviousParametersSet);

        /// <summary>
        ///     Gets a value indicating whether the current parameters slot is set.
        /// </summary>
        public bool IsCurrentSet => parameters.Status.HasFlag(StateMachineParametersFlags.CurrentParametersSet);

        /// <summary>
        ///     Gets a value indicating whether the next parameters slot is set.
        /// </summary>
        public bool IsNextSet => parameters.Status.HasFlag(StateMachineParametersFlags.NextParametersSet);
    }
}
