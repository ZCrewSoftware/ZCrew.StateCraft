namespace OrderProcessor.Domain.Models;

public class Order
{
    public Guid Id { get; set; }
    public OrderState State { get; set; } = OrderState.Queued;
    public List<Line> Lines { get; init; } = [];
    public string? SuspensionReason { get; set; }
    public string? CancellationReason { get; set; }
}
