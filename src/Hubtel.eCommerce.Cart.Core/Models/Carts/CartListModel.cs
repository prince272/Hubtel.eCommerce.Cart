using Hubtel.eCommerce.Cart.Core.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Core.Models.Carts
{
    public class CartListModel
    {
        public IEnumerable<CartModel> Items { get; set; }
    }

    public class CartPageModel : CartListModel, IPageable<CartModel>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public long TotalItems { get; set; }

        public int TotalPages { get; set; }
    }
}
