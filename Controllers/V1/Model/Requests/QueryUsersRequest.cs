﻿using NetCoreExampleApi.Models.Pagination;

namespace NetCoreExampleApi.Controllers.V1.Model.Requests
{
    public class QueryUsersRequest : PagedRequest
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Surename { get; set; }
    }
}

