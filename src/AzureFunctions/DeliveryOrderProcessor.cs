using AzureFunctions.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions;
public class DeliveryOrderProcessor
{
    private readonly ILogger<DeliveryOrderProcessor> _logger;

    public DeliveryOrderProcessor(ILogger<DeliveryOrderProcessor> logger)
    {
        _logger = logger;
    }

    [Function("DeliveryOrderProcessor")]
    public async Task<DeliveryOrderProcessorResponse> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        string order = await new StreamReader(req.Body).ReadToEndAsync();

        return new DeliveryOrderProcessorResponse
        {
            Order = order,
            HttpResponse = new OkObjectResult("Azure function has been executed.")
        };
    }
}
