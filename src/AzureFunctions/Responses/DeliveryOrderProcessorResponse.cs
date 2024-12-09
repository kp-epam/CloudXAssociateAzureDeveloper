using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Responses;
public class DeliveryOrderProcessorResponse
{
    [CosmosDBOutput("%CosmosDb%", "%CosmosContainerOut%", Connection = "CosmosDbConnectionSetting", CreateIfNotExists = true)]
    public string Order { get; set; }

    [HttpResult]
    public IActionResult HttpResponse { get; set; }
}
