using FluentValidation;
using Hubtel.eCommerce.Cart.Core.Models.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Core.Models.Carts
{
    public class AddItemToCartForm
    {
        public long ItemId { get; set; }

        public int Quantity { get; set; }

        public class Validator : AbstractValidator<AddItemToCartForm>
        {
            public Validator()
            {
                RuleFor(m => m.Quantity).InclusiveBetween(0, int.MaxValue);
            }
        }
    }
}
