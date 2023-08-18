using FluentValidation;
using Hubtel.eCommerce.Cart.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hubtel.eCommerce.Cart.Core.Models.Accounts
{
    public class SignUpForm
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        [JsonIgnore]
        public ContactType UsernameType => TextHelper.GetContactType(Username);

        public string Password { get; set; }

        public class Validator : AbstractValidator<SignUpForm>
        {
            public Validator()
            {
                RuleFor(m => m.FirstName).NotEmpty();
                RuleFor(m => m.LastName).NotEmpty();
                RuleFor(m => m.Username).NotEmpty().Username();
                RuleFor(m => m.Password).NotEmpty().Password();
            }
        }
    }
}