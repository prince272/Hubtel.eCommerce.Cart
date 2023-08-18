using FluentValidation;
using Hubtel.eCommerce.Cart.Core.Models.Carts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Items
{
    public class GetItemFilter
    {
        public long Id { get; set; }

        public class Validator : AbstractValidator<GetItemFilter>
        {
            public Validator()
            {
            }
        }
    }

    public class GetItemPageFilter
    {
        public long[] Ids { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public class Validator : AbstractValidator<GetItemPageFilter>
        {
            public Validator()
            {
                RuleFor(m => m.PageNumber).InclusiveBetween(1, int.MaxValue);
                RuleFor(m => m.PageSize).InclusiveBetween(1, int.MaxValue);
            }
        }
    }
}
