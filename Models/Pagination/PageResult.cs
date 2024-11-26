using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreExampleApi.Models.Pagination
{
    public class PageResult<T> : OkObjectResult
    {
        private readonly IPage<T> _page;

        public PageResult(IPage<T> page) : base(page.Items)
        {
            _page = page;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            SetHeaders(context);

            await base.ExecuteResultAsync(context);
        }

        private void SetHeaders(ActionContext context)
        {
            context.HttpContext.Response.Headers.Append("x-page", _page.ToString());
            context.HttpContext.Response.Headers.Append("x-page-index", _page.Index.ToString());
            context.HttpContext.Response.Headers.Append("x-page-size", _page.Size.ToString());
            context.HttpContext.Response.Headers.Append("x-total-records", _page.TotalCount.ToString());
            context.HttpContext.Response.Headers.Append("x-total-pages", _page.TotalPages.ToString());
            context.HttpContext.Response.Headers.Append("x-hasPreviousPage", _page.HasPreviousPage.ToString().ToLowerInvariant());
            context.HttpContext.Response.Headers.Append("Link", GetLinkHeaderForPageResult(context.HttpContext.Request, _page));
        }

        private string GetLinkHeaderForPageResult(HttpRequest request, IPage<T> page)
        {
            string requestUrl = request.GetDisplayUrl();
            string headerValue = string.Empty;

            if (page.HasNextPage)
            {
                string nextPageUrl = requestUrl.ToLowerInvariant().Replace($"page={page.Index}", $"page={page.Index + 1}");
                headerValue += $"<{nextPageUrl}>; rel=\"next\",";
            }

            string lastPageUrl = requestUrl.ToLowerInvariant().Replace($"page={page.Index}", $"page={page.TotalPages}");
            headerValue += $"<{lastPageUrl}>; rel=\"last\",";

            string firstPageUrl = requestUrl.ToLowerInvariant().Replace($"page={page.Index}", $"page=1");
            headerValue += $"<{firstPageUrl}>; rel=\"first\",";

            if (page.HasPreviousPage)
            {
                string previousPageUrl = requestUrl.ToLowerInvariant().Replace($"page={page.Index}", $"page={page.Index - 1}");
                headerValue += $"<{previousPageUrl}>; rel=\"prev\",";
            }

            if (!string.IsNullOrEmpty(headerValue))
            {
                headerValue = headerValue.TrimEnd(',');
            }

            return headerValue;
        }
    }
}

