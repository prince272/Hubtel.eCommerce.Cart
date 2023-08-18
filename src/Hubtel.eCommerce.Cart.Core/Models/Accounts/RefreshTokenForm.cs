using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Core.Models.Accounts
{
    public class RefreshSessionForm
    {
        public string RefreshToken { get; set; } = default!;

        public class Validator : AbstractValidator<RefreshSessionForm>
        {
            public Validator()
            {
                RuleFor(_ => _.RefreshToken).NotEmpty();
            }
        }
    }
}
