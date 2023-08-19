using Hubtel.eCommerce.Cart.Core.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Core.Entities
{
    public class Cart : IEntity
    {
        public long Id { get; set; }

        public virtual User User { get; set; }

        public long UserId { get; set; }    

        public Item Item { get; set; }

        public long ItemId { get; set; }

        public int Quantity { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
