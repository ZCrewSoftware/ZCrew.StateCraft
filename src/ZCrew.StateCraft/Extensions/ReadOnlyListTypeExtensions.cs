namespace ZCrew.StateCraft;

/// <summary>
///     Provides extension methods for comparing lists of <see cref="Type"/>
///     for pairwise assignability.
/// </summary>
internal static class ReadOnlyListTypeExtensions
{
    extension(IReadOnlyList<Type> types)
    {
        /// <summary>
        ///     Determines whether each type in this list is assignable from the
        ///     corresponding type in <paramref name="otherTypes"/>, checking
        ///     element-by-element. Returns <see langword="false"/> if the lists
        ///     differ in length.
        /// </summary>
        /// <param name="otherTypes">The list of types to check against.</param>
        /// <returns>
        ///     <see langword="true"/> if the lists are the same length and every
        ///     element in <paramref name="otherTypes"/> is assignable to the
        ///     corresponding element in this list; otherwise,
        ///     <see langword="false"/>.
        /// </returns>
        internal bool IsAssignableFrom(IReadOnlyList<Type> otherTypes)
        {
            if (types.Count != otherTypes.Count)
            {
                return false;
            }

            for (var i = 0; i < types.Count; i++)
            {
                if (!types[i].IsAssignableFrom(otherTypes[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
