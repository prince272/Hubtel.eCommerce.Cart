using FluentValidation;
using Hubtel.eCommerce.Cart.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Accounts
{
    public class SignInForm
    {
        public string Username { get; set; }

        [JsonIgnore]
        public ContactType UsernameType => TextHelper.GetContactType(Username);

        public string Password { get; set; }

        public class Validator : AbstractValidator<SignInForm>
        {
            public Validator()
            {
                RuleFor(m => m.Username).NotEmpty().Username();
                RuleFor(m => m.Password).NotEmpty();
            }
        }
    }
}
