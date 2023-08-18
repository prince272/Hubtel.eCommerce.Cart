using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Carts
{
    public class GetCartFilter
    {
        public long Id { get; set; }

        public class Validator : AbstractValidator<GetCartFilter>
        {
            public Validator()
            {
            }
        }
    }

    public class GetCartPageFilter
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public long[] Ids { get; set; }

        public long[] ItemIds { get; set; }

        public int? Quantity { get; set; }

        public TimeSpan? Time { get; set; }

        public string[] PhoneNumbers { get; set; }

        public class Validator : AbstractValidator<GetCartPageFilter>
        {
            public Validator()
            {
                RuleFor(m => m.PageNumber).InclusiveBetween(1, int.MaxValue);
                RuleFor(m => m.PageSize).InclusiveBetween(1, int.MaxValue);
            }
        }
    }
}