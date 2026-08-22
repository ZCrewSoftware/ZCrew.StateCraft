namespace ZCrew.StateCraft.PlantUml;

/// <summary>
///     How newline characters inside encoded descriptors are handled when rendered into a PlantUML diagram.
/// </summary>
public enum PlantUmlNewline
{
    /// <summary>
    ///     Strip newline characters entirely; subsequent text is concatenated against the preceding line.
    /// </summary>
    Ignore,

    /// <summary>
    ///     Replace each newline with a single space so descriptors stay on one PlantUML line.
    /// </summary>
    Space,

    /// <summary>
    ///     Replace each newline with PlantUML's <c>\n</c> escape so descriptors render as multiple lines in the
    ///     diagram.
    /// </summary>
    LineBreak,
}
