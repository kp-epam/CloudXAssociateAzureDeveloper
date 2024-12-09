using System.Text.Json.Serialization;

namespace AzureFunctionsShared.OrderItemsReserver;
public class LogicAppRequest
{
    [JsonPropertyName("messageId")]
    public string MessageId { get; set; }

    [JsonPropertyName("orderItems")]
    public ReservedOrderItems OrderItems { get; set; }
}
