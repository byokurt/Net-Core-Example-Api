namespace DotnetCoreExampleApi.Models.Pagination
{
	public class PagedRequest
	{
		public int Page { get; set; }

		public int PageSize { get; set; }

		public string OrderBy { get; set; }

		public PaginationOrderType Order { get; set; }

	}
}

