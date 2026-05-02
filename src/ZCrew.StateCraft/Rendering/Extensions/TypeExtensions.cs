namespace ZCrew.StateCraft.Rendering.Extensions;

/// <summary>
///     Rendering helpers exposed on <see cref="Type"/>.
/// </summary>
internal static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        ///     A stable identifier for the type suitable for use as a node identifier in a rendered diagram. Returns
        ///     <see cref="Type.FullName"/> when available and falls back to
        ///     <see cref="System.Reflection.MemberInfo.Name"/> for types that do not expose a full name (for example,
        ///     generic type parameters).
        /// </summary>
        public string RenderingIdentifier
        {
            get { return type.FullName ?? type.Name; }
        }
    }
}
