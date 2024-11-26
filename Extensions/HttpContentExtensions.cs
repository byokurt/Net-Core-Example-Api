using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetCoreExampleApi.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter()
                }
        };

        T value = await content.ReadFromJsonAsync<T>(jsonSerializerOptions);

        return value;
    }
}