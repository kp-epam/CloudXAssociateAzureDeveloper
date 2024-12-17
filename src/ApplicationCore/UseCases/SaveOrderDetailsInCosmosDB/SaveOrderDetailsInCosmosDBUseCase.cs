using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AzureFunctionsShared.DeliveryOrderProcessor;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.eShopWeb.ApplicationCore.UseCases.SaveOrderDetailsInCosmosDB;
public class SaveOrderDetailsInCosmosDBUseCase : ISaveOrderDetailsInCosmosDBUseCase
{
    private readonly ILogger<SaveOrderDetailsInCosmosDBUseCase> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public SaveOrderDetailsInCosmosDBUseCase(
        ILogger<SaveOrderDetailsInCosmosDBUseCase> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }


    public async Task Apply(string? azureFunctionUrl, Order order)
    {
        ValidateParameters(azureFunctionUrl, order);

        var orderInfo = MapOrderEntity(order);
        var orderInfoJson = JsonSerializer.Serialize(orderInfo);
        var requestContent = new StringContent(orderInfoJson, Encoding.UTF8, Application.Json);
        var request = new HttpRequestMessage(HttpMethod.Post, azureFunctionUrl)
        {
            Content = requestContent
        };

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        _logger.LogInformation("Order has been sent.");
    }


    private void ValidateParameters(string? azureFunctionUrl, Order order)
    {
        if (string.IsNullOrEmpty(azureFunctionUrl))
        {
            throw new ArgumentException("Azure function url cannot be empty.", nameof(azureFunctionUrl));
        }

        if (order == null)
        {
            throw new ArgumentNullException(nameof(order), "Order cannot be null.");
        }
    }

    private OrderInfo MapOrderEntity(Order order)
    {
        var address = MapAddressEntity(order.ShipToAddress);
        var items = MapOrderItems(order.OrderItems);

        return new OrderInfo
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            ShippingAddress = address,
            Items = items,
            FinalPrice = order.Total()
        };
    }

    private AddressInfo MapAddressEntity(Address address)
    {
        return new AddressInfo
        {
            Street = address.Street,
            City = address.City,
            Country = address.Country,
            State = address.State,
            ZipCode = address.ZipCode
        };
    }

    private List<OrderItemInfo> MapOrderItems(IReadOnlyCollection<OrderItem> items)
    {
        return items
            .Select(item => new OrderItemInfo
            {
                CatalogItem = new CatalogItemInfo
                {
                    Id = item.ItemOrdered.CatalogItemId,
                    ProductName = item.ItemOrdered.ProductName,
                    PictureUri = item.ItemOrdered.PictureUri
                },
                UnitPrice = item.UnitPrice,
                Units = item.Units
            }).ToList();
    }
}
