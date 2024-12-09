using System.Text.Json.Serialization;

namespace AzureFunctionsShared.OrderItemsReserver;
public class ReservedOrderItem
{
    [JsonPropertyName("itemId")]
    public int Id { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
