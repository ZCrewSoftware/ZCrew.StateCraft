namespace ZCrew.StateCraft;

internal static class ReadOnlyListTypeExtensions
{
    extension(IReadOnlyList<Type> types)
    {
        internal bool IsAssignableFrom(ReadOnlySpan<Type> otherTypes)
        {
            if (types.Count != otherTypes.Length)
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
