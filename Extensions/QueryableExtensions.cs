using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DotnetCoreExampleApi.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoreExampleApi.Extensions
{
	public static class QueryableExtensions
	{
		public static async Task<IPage<T>> ToPageAsync<T>(this IQueryable<T> source, PagedRequest request)
		{
			if (source == null)
			{
				throw new ArgumentException(nameof(source));
			}

			if (request == null)
			{
                throw new ArgumentException(nameof(request));
            }

			if (string.IsNullOrEmpty(request.OrderBy))
			{
				throw new ApplicationException("In order to use paging extensions you need to supply an OrderBy parameter.");
			}

			if (request.Order == PaginationOrderType.Asc)
			{
				source = source.OrderBy(request.OrderBy);
			}
			else if (request.Order == PaginationOrderType.Desc)
			{
				source = source.OrderBy(request.OrderBy + "descending");
			}

			int skip = (request.Page - 1) * request.PageSize;
			int take = request.PageSize;
			int totalItemCount = await source.CountAsync();

			List<T> items = await source.Skip(skip).Take(take).ToListAsync();

			return new Page<T>(items, request.Page, request.PageSize, totalItemCount);
        }
	}
}

