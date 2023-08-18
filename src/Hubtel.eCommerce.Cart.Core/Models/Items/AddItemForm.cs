using FluentValidation;
using Hubtel.eCommerce.Cart.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Items
{
    public class AddItemForm
    {
        [JsonIgnore]
        public long Id { get; set; }

        public class Validator : AbstractValidator<AddItemForm>
        {
            public Validator()
            {
            }
        }
    }
}
