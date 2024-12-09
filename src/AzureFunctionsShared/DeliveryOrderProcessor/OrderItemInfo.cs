using System.Text.Json.Serialization;

namespace AzureFunctionsShared.DeliveryOrderProcessor;
public class OrderItemInfo
{
    [JsonPropertyName("catalogItem")]
    public CatalogItemInfo? CatalogItem { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("units")]
    public int Units { get; set; }
}
