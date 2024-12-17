using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureFunctionsShared.OrderItemsReserver;

namespace Microsoft.eShopWeb.ApplicationCore.UseCases.PushOrderItemsToServiceBus;
public class PushOrderItemsToServiceBusUseCase : IPushOrderItemsToServiceBusUseCase
{
    public async Task Apply(string? serviceBusConnString,
        string? serviceBusQueueName, IDictionary<int, int> basketQuantities)
    {
        ValidateParameters(serviceBusConnString, serviceBusQueueName, basketQuantities);

        var reservedOrderItems = MapBasketQuantities(basketQuantities);
        var messageContent = JsonSerializer.Serialize(reservedOrderItems);

        await using (var client = new ServiceBusClient(serviceBusConnString))
        {
            var sender = client.CreateSender(serviceBusQueueName);
            var message = new ServiceBusMessage(messageContent);

            await sender.SendMessageAsync(message);
        }
    }


    private void ValidateParameters(string? serviceBusConnString, string? serviceBusQueueName, IDictionary<int, int> basketQuantities)
    {
        if (string.IsNullOrEmpty(serviceBusConnString))
        {
            throw new ArgumentException("Service bus connection string cannot be empty.", nameof(serviceBusConnString));
        }

        if (string.IsNullOrEmpty(serviceBusQueueName))
        {
            throw new ArgumentException("Service bus queue name cannot be empty.", nameof(serviceBusQueueName));
        }

        if (basketQuantities == null)
        {
            throw new ArgumentNullException(nameof(basketQuantities), "Basket quantities cannot be null.");
        }

        if (basketQuantities.Count < 1)
        {
            throw new ArgumentException("Basket quantities cannot be empty.", nameof(basketQuantities));
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
