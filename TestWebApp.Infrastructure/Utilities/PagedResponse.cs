using System.Collections.Generic;
using System.Linq;

namespace TestWebApp.Infrastructure.Utilities
{
    public class PagedResponse<T>
    {
        public PagedResponse(IEnumerable<T> data, int pageIndex, int pageSize)
        {
            Data = data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            Total = data.Count();
        }

        public int Total { get; set; }
        public ICollection<T> Data { get; set; }
    }
}
