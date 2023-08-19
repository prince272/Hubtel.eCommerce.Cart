using AbstractProfile = AutoMapper.Profile;
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Extensions.Identity;
using Hubtel.eCommerce.Cart.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Core.Models.Carts
{
    public class CartModel
    {
        public long UserId { get; set; }

        public long ItemId { get; set; }

        public string ItemName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Amount { get; set; }

        public class Profile : AbstractProfile
        {
            public Profile()
            {
                CreateMap<Entities.Cart, CartModel>();
            }
        }
    }
}
