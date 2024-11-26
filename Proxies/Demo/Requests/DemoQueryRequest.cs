using System.Text.Json.Serialization;

namespace DotnetCoreExampleApi.Proxies.Demo.Requests;

public class DemoQueryRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}