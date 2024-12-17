using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.UseCases.PushOrderItemsToServiceBus;
public interface IPushOrderItemsToServiceBusUseCase
{
    /// <summary>
    /// Send data to Azure Service Bus.
    /// </summary>
    /// <param name="serviceBusConnString"></param>
    /// <param name="serviceBusQueueName"></param>
    /// <param name="basketQuantities"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task Apply(string? serviceBusConnString,
        string? serviceBusQueueName, IDictionary<int, int> basketQuantities);
}
