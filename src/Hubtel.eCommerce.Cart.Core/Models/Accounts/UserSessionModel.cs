﻿using AbstractProfile = AutoMapper.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Extensions.Identity;

namespace NextSolution.Core.Models.Accounts
{
    public class UserSessionModel
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string TokenType { get; set; }

        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();

        public class Profile : AbstractProfile
        {
            public Profile()
            {
                CreateMap<User, UserSessionModel>();
                CreateMap<UserSessionInfo, UserSessionModel>();
            }
        }
    }
}

