using System.Text.Json.Serialization;

namespace AzureFunctionsShared.DeliveryOrderProcessor;
public class CatalogItemInfo
{
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("productName")]
    public string? ProductName { get; set; }

    [JsonPropertyName("pictureUri")]
    public string? PictureUri { get; set; }
}
