using System.Text.Json.Serialization;

namespace NetCoreExampleApi.Proxies.Demo.Requests;

public class DemoCreateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}