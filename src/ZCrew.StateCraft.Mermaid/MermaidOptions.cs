namespace ZCrew.StateCraft.Mermaid;

/// <summary>
///     Configuration options that control how a state machine is rendered as a Mermaid diagram.
/// </summary>
public class MermaidOptions
{
    /// <summary>
    ///     The layout direction the diagram should use. Defaults to <see cref="MermaidDirection.TopToBottom"/>.
    /// </summary>
    public MermaidDirection Direction { get; set; } = MermaidDirection.TopToBottom;

    /// <summary>
    ///     The strategy used when newline characters are encountered inside an encoded descriptor. Defaults to
    ///     <see cref="MermaidNewline.Ignore"/>.
    /// </summary>
    public MermaidNewline Newline { get; set; } = MermaidNewline.Ignore;
}
