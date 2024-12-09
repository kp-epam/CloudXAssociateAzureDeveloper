using System.Text.Json.Serialization;

namespace AzureFunctionsShared.DeliveryOrderProcessor;
public class AddressInfo
{
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("zipCode")]
    public string? ZipCode { get; set; }
}
