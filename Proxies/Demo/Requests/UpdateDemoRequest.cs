using System.Text.Json.Serialization;

namespace NetCoreExampleApi.Proxies.Demo.Requests;

public class UpdateDemoRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}