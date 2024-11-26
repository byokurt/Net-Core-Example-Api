using System.Collections.Generic;

namespace NetCoreExampleApi.Proxies.Demo.Responses;

public class DemoQueryResponse
{
    public int TotalRecords { get; set; }

    public int TotalPages { get; set; }

    public List<DemoQueryItemResponse> QueryItems { get; set; }
}