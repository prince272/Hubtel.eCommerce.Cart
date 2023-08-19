using AbstractProfile = AutoMapper.Profile;
using Hubtel.eCommerce.Cart.Core.Models.Carts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Core.Models.Items
{
    public class GetItemModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public class Profile : AbstractProfile
        {
            public Profile()
            {
                CreateMap<Entities.Item, GetItemModel>();
            }
        }
    }
}
