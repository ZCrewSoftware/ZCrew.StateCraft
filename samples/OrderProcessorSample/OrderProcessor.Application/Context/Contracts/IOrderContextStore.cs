using OrderProcessor.Domain.Models;

namespace OrderProcessor.Application.Context.Contracts;

/// <remarks>
///     Since this is just a sample the full life-cycle management is excluded and contexts will stick around forever.
/// </remarks>
internal interface IOrderContextStore
{
    ValueTask<IOrderContext> CreateOrderContext(Order order, CancellationToken token);
    ValueTask<IOrderContext> GetOrderContext(Guid orderId, CancellationToken token);
}
