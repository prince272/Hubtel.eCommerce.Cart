using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Shared
{
    public interface IPageable<T>
    {
        int PageNumber { get; }
        int PageSize { get; }
        long TotalItems { get; }
        int TotalPages { get; }
        IEnumerable<T> Items { get; }
    }
}
