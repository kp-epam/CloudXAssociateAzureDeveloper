using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.UseCases.PushOrderItemsToServiceBus;
public interface IPushOrderItemsToServiceBusUseCase
{
    Task Apply(IDictionary<int, int> basketQuantities);
}
