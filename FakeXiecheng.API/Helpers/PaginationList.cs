using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FakeXiecheng.API.Helpers
{
    public class PaginationList<T> : List<T>
    {
        public PaginationList(
            int currentPage,
            int pageSie,
            IEnumerable<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSie;
            AddRange(items);
        }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public static async Task<PaginationList<T>> Create(int currentPage, int pageSize, IQueryable<T> query)
        {
            var skip = (currentPage - 1) * pageSize;
            query = query.Skip(skip);
            query = query.Take(pageSize);

            var items = await query.ToArrayAsync();
            return new PaginationList<T>(currentPage, pageSize, items);
        }
    }
}