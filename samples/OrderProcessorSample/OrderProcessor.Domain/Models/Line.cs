namespace OrderProcessor.Domain.Models;

public class Line
{
    public Guid Id { get; set; }
    public LineState State { get; set; } = LineState.Incomplete;
    public string? SuspensionReason { get; set; }
    public string? CancellationReason { get; set; }
}
