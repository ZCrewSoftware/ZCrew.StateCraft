namespace ZCrew.StateCraft.PlantUml;

/// <summary>
///     Configuration options that control how a state machine is rendered as a PlantUML diagram.
/// </summary>
public class PlantUmlOptions
{
    /// <summary>
    ///     The layout direction the diagram should use. Defaults to <see cref="PlantUmlDirection.TopToBottom"/>.
    /// </summary>
    public PlantUmlDirection Direction { get; set; } = PlantUmlDirection.TopToBottom;

    /// <summary>
    ///     The strategy used when newline characters are encountered inside an encoded descriptor. Defaults to
    ///     <see cref="PlantUmlNewline.Ignore"/>.
    /// </summary>
    public PlantUmlNewline Newline { get; set; } = PlantUmlNewline.Ignore;
}
