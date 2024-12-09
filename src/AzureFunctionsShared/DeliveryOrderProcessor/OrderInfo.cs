using System.Text.Json.Serialization;

namespace AzureFunctionsShared.DeliveryOrderProcessor;
public class OrderInfo
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("shippingAddress")]
    public AddressInfo? ShippingAddress { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItemInfo>? Items { get; set; }

    [JsonPropertyName("finalPrice")]
    public decimal FinalPrice { get; set; }
}
