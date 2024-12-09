using System.Text.Json.Serialization;

namespace AzureFunctionsShared.OrderItemsReserver;
public class ReservedOrderItems
{
    [JsonPropertyName("items")]
    public required IList<ReservedOrderItem> Items { get; set; }
}
