using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Carts
{
    public class RemoveItemFromCartForm
    {
        public long ItemId { get; set; }

        public class Validator : AbstractValidator<RemoveItemFromCartForm>
        {
            public Validator()
            {
            }
        }
    }
}
