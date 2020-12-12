using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FakeXiecheng.API.Helpers
{
    public class PaginationList<T> : List<T>
    {
        public PaginationList(
            int totalCount,
            int currentPage,
            int pageSie,
            IEnumerable<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSie;
            AddRange(items);
            TotalCount = totalCount;
            TotalPage = (int) Math.Ceiling(totalCount / (double) pageSie);
        }

        public int TotalPage { get; }

        public int TotalCount { get; }

        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPage;

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public static async Task<PaginationList<T>> Create(int currentPage, int pageSize, IQueryable<T> query)
        {
            var totalCount = await query.CountAsync();
            
            var skip = (currentPage - 1) * pageSize;
            query = query.Skip(skip);
            query = query.Take(pageSize);

            var items = await query.ToArrayAsync();
            return new PaginationList<T>(totalCount, currentPage, pageSize, items);
        }
    }
}