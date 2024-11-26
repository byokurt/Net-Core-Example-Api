using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotnetCoreExampleApi.Extensions;
using DotnetCoreExampleApi.Proxies.Demo.Requests;
using DotnetCoreExampleApi.Proxies.Demo.Responses;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetCoreExampleApi.Proxies.Demo;

public class DemoApiProxy : IDemoApiProxy
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DemoApiProxy> _logger;

    public DemoApiProxy(HttpClient httpClient, ILogger<DemoApiProxy> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DemoQueryResponse> QueryTransaction(DemoQueryRequest request)
    {
        string queryParams = request.ToQueryString();

        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"v1/gifts?{queryParams}");

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            _logger.LogError("There was a problem while getting data from Demo Api. ResponseStatusCode:{ResponseStatusCode} ResponseBody:{ResponseBody}", (int)httpResponseMessage.StatusCode, responseContent);

            throw new ApplicationException($"There was a problem while getting data from Demo Api ResponseStatusCode:{(int)httpResponseMessage.StatusCode} ResponseBody:{responseContent}");
        }

        DemoQueryResponse demoResponse = new DemoQueryResponse()
        {
            TotalRecords = Convert.ToInt32(httpResponseMessage.Headers.GetValues("x-total-pages").FirstOrDefault()),
            TotalPages = Convert.ToInt32(httpResponseMessage.Headers.GetValues("x-total-records").FirstOrDefault()),
            QueryItems = await httpResponseMessage.Content.ReadAsJsonAsync<List<DemoQueryItemResponse>>()
        };

        return demoResponse;
    }

    public async Task<DemoGetResponse> GetUser(int id)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"v1/gifts/{id}");

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            _logger.LogError("There was a problem while getting data from Demo Api. ResponseStatusCode:{ResponseStatusCode} ResponseBody:{ResponseBody}", (int)httpResponseMessage.StatusCode, responseContent);

            throw new ApplicationException($"There was a problem while getting data from Demo Api ResponseStatusCode:{(int)httpResponseMessage.StatusCode} ResponseBody:{responseContent}");
        }

        return await httpResponseMessage.Content.ReadAsJsonAsync<DemoGetResponse>();
    }

    public async Task UpdateUser(int id, UpdateDemoRequest request)
    {
        JsonPatchDocument document = new JsonPatchDocument();

        document.Replace(nameof(request.Name), request.Name);

        string requestModel = JsonSerializer.Serialize(document.Operations);

        StringContent httpContent = new StringContent(requestModel, Encoding.UTF8, "application/json-patch+json");

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"v1/demo/{id}")
        {
            Content = httpContent
        };

        HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

        if (!responseMessage.IsSuccessStatusCode)
        {
            ProblemDetails problemDetails = await responseMessage.Content.ReadAsJsonAsync<ProblemDetails>();

            if (problemDetails != null)
            {
                _logger.LogError($"Patch api call failed. Status Code: {problemDetails.Status}");
            }
        }
    }

    public async Task<DemoCreateResponse> CreateUser(DemoCreateRequest request)
    {
        string requestModel = JsonSerializer.Serialize(request);

        StringContent stringContent = new StringContent(requestModel, Encoding.UTF8, MediaTypeNames.Application.Json);

        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"v1/users")
        {
            Content = stringContent
        };

        HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            _logger.LogError("There was a problem while getting data from Demo Api. ResponseStatusCode:{ResponseStatusCode} ResponseBody:{ResponseBody}", (int)httpResponseMessage.StatusCode, responseContent);

            throw new ApplicationException($"There was a problem while setting data from Demo Api ResponseStatusCode:{(int)httpResponseMessage.StatusCode} ResponseBody:{responseContent}");
        }

        return await httpResponseMessage.Content.ReadAsJsonAsync<DemoCreateResponse>();
    }
}