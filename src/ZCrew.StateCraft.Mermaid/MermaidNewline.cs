namespace ZCrew.StateCraft.Mermaid;

/// <summary>
///     How newline characters inside encoded descriptors are handled when rendered into a Mermaid diagram.
/// </summary>
public enum MermaidNewline
{
    /// <summary>
    ///     Strip newline characters entirely; subsequent text is concatenated against the preceding line.
    /// </summary>
    Ignore,

    /// <summary>
    ///     Replace each newline with a single space so descriptors stay on one Mermaid line.
    /// </summary>
    Space,

    /// <summary>
    ///     Replace each newline with the HTML <c>&lt;br/&gt;</c> tag so descriptors render as multiple lines in
    ///     the diagram.
    /// </summary>
    HtmlSingleLineBreak,
}
