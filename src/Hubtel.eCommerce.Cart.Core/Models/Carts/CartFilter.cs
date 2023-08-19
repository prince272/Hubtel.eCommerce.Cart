using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Carts
{
    public class CartFilter
    {
        public long Id { get; set; }

        public class Validator : AbstractValidator<CartFilter>
        {
            public Validator()
            {
            }
        }
    }

    public class CartListFilter
    {

        public long[] Ids { get; set; }

        public long[] ItemIds { get; set; }

        public int? Quantity { get; set; }

        public TimeSpan? Time { get; set; }

        public string[] PhoneNumbers { get; set; }

        public class Validator : AbstractValidator<CartListFilter>
        {
            public Validator()
            {
            }
        }
    }

    public class CartPageFilter : CartListFilter
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public new class Validator : AbstractValidator<CartPageFilter>
        {
            public Validator()
            {
                RuleFor(m => m.PageNumber).InclusiveBetween(1, int.MaxValue);
                RuleFor(m => m.PageSize).InclusiveBetween(1, int.MaxValue);
            }
        }
    }
}