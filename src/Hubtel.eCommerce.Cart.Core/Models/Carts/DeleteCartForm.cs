using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Carts
{
    public class DeleteCartForm
    {
        public long Id { get; set; }

        public class Validator : AbstractValidator<DeleteCartForm>
        {
            public Validator()
            {
            }
        }
    }
}
