using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Items
{
    public class DeleteItemForm
    {
        [JsonIgnore]
        public long Id { get; set; }

        public class Validator : AbstractValidator<DeleteItemForm>
        {
            public Validator()
            {
            }
        }
    }
}
