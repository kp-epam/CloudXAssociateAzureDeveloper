using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using AzureFunctionsShared.OrderItemsReserver;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace AzureFunctions;
public class OrderItemsReserver
{
    private static HttpClient httpClient = new();

    private readonly ILogger<OrderItemsReserver> _logger;

    public OrderItemsReserver(ILogger<OrderItemsReserver> logger)
    {
        _logger = logger;
    }

    [Function(nameof(OrderItemsReserver))]
    public async Task Run(
        [ServiceBusTrigger("orderitemsreserver", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        string blobConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnection") ?? string.Empty;
        string blobContainerName = Environment.GetEnvironmentVariable("BlobContainerName") ?? string.Empty;
        string logicAppRequestEndpoint = Environment.GetEnvironmentVariable("LogicAppRequestEndpoint") ?? string.Empty;

        try
        {
            if (message.Body != null)
            {
                var blobOptions = new BlobClientOptions()
                {
                    Retry = {
                        Mode = RetryMode.Exponential,
                        MaxRetries = 3,
                        Delay = TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(10),
                        NetworkTimeout = TimeSpan.FromSeconds(100)
                    }
                };

                var blobName = $"{message.MessageId}.json";

                var blobServiceClient = new BlobServiceClient(blobConnectionString, blobOptions);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
                var blobClient = blobContainerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(message.Body, overwrite: true);
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, exc.Message);
            await CallAzureLogicApp(message, logicAppRequestEndpoint);
        }

        await messageActions.CompleteMessageAsync(message);
    }

    private async Task CallAzureLogicApp(ServiceBusReceivedMessage message, string endpoint)
    {
        var orderItems = JsonSerializer.Deserialize<ReservedOrderItems>(message.Body);
        if (orderItems == null)
        {
            return;
        }

        var logicAppRequest = new LogicAppRequest
        {
            MessageId = message.MessageId,
            OrderItems = orderItems
        };

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(logicAppRequest), Encoding.UTF8, Application.Json)
        };

        await httpClient.SendAsync(request);
    }
}
