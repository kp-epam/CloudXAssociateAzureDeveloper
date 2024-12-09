using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureFunctionsShared.OrderItemsReserver;

namespace Microsoft.eShopWeb.ApplicationCore.UseCases.PushOrderItemsToServiceBus;
public class PushOrderItemsToServiceBusUseCase : IPushOrderItemsToServiceBusUseCase
{
    private const string ServiceBusConnectionString = "";
    private const string ServiceBusQueueName = "";

    public async Task Apply(IDictionary<int, int> basketQuantities)
    {
        if (basketQuantities == null || basketQuantities.Count < 1)
        {
            return;
        }

        var reservedOrderItems = MapBasketQuantities(basketQuantities);
        var messageContent = JsonSerializer.Serialize(reservedOrderItems);

        await using (var client = new ServiceBusClient(ServiceBusConnectionString))
        {
            var sender = client.CreateSender(ServiceBusQueueName);
            var message = new ServiceBusMessage(messageContent);

            await sender.SendMessageAsync(message);
        }
    }


    private ReservedOrderItems MapBasketQuantities(IDictionary<int, int> basketItems)
    {
        var orderItems = basketItems.Select(x => new ReservedOrderItem
        {
            Id = x.Key,
            Quantity = x.Value,
        }).ToList();

        return new ReservedOrderItems
        {
            Items = orderItems
        };
    }
}
