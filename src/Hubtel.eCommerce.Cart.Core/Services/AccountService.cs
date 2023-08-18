using AutoMapper;
using FluentValidation;
using Hubtel.eCommerce.Cart.Core.Constants;
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Exceptions;
using Hubtel.eCommerce.Cart.Core.Models.Accounts;
using Hubtel.eCommerce.Cart.Core.Repositories;
using Hubtel.eCommerce.Cart.Core.Shared;
using Hubtel.eCommerce.Cart.Core.Utilities;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Services
{
    public class AccountService : IService
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _validatorProvider;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public AccountService(IMapper mapper, IServiceProvider validatorProvider, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task SignUpAsync(SignUpForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<SignUpForm.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(form);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            // Ensures that the email is already in use.
            var user = await new Func<Task<User>>(() =>
            {

                if (form.UsernameType == ContactType.EmailAddress) return _userRepository.FindByEmailAsync(form.Username);
                else if (form.UsernameType == ContactType.PhoneNumber) return _userRepository.FindByPhoneNumberAsync(form.Username);
                else throw new InvalidOperationException();
            })();

            if (user != null) throw new BadRequestException(nameof(form.Username), $"'{form.Username.Humanize(LetterCasing.Title)}' is already in use.");

            user = new User();
            user.FirstName = form.FirstName;
            user.LastName = form.LastName;
            user.Email = form.UsernameType == ContactType.EmailAddress ? form.Username : user.Email;
            user.PhoneNumber = form.UsernameType == ContactType.PhoneNumber ? form.Username : user.PhoneNumber;
            user.Active = true;
            user.ActiveAt = DateTimeOffset.UtcNow;
            await _userRepository.GenerateUserNameAsync(user);
            await _userRepository.CreateAsync(user, form.Password);

            foreach (var roleName in Roles.All)
            {
                if (await _roleRepository.FindByNameAsync(roleName) is null)
                    await _roleRepository.CreateAsync(new Role(roleName));
            }

            var totalUsers = await _userRepository.CountAsync();

            // Assign roles to the specified user based on the total user count.
            // If there is only one user, grant both Admin and Member roles.
            // Otherwise, assign only the Member role.
            await _userRepository.AddToRolesAsync(user, (totalUsers == 1) ? new[] { Roles.Admin, Roles.Member } : new[] { Roles.Member });
        }

        public async Task<UserSessionModel> SignInAsync(SignInForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<SignInForm.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(form);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            var user = await new Func<Task<User>>(() => {

                if (form.UsernameType == ContactType.EmailAddress) return _userRepository.FindByEmailAsync(form.Username);
                else if (form.UsernameType == ContactType.PhoneNumber) return _userRepository.FindByPhoneNumberAsync(form.Username);
                else throw new InvalidOperationException();
            })();
            if (user == null) throw new BadRequestException(nameof(form.Username), $"'{form.Username.Humanize(LetterCasing.Title)}' does not exist.");

            if (!await _userRepository.CheckPasswordAsync(user, form.Password))
                throw new BadRequestException(nameof(form.Password), $"'{nameof(form.Password).Humanize(LetterCasing.Title)}' is not correct.");

            var session = await _userRepository.GenerateSessionAsync(user);
            await _userRepository.AddSessionAsync(user, session);

            var model = _mapper.Map(user, _mapper.Map<UserSessionModel>(session));
            model.Roles = await _userRepository.GetRolesAsync(user);
            return model;
        }

        public async Task<UserSessionModel> RefreshSessionAsync(RefreshSessionForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<RefreshSessionForm.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(form);

            if (!formValidationResult.IsValid)
                throw new BadRequestException(formValidationResult.ToDictionary());

            var user = await _userRepository.FindByRefreshTokenAsync(form.RefreshToken);

            if (user == null) throw new BadRequestException(nameof(form.RefreshToken), $"'{nameof(form.RefreshToken).Humanize(LetterCasing.Title)}' is not valid.");

            await _userRepository.RemoveSessionAsync(user, form.RefreshToken);

            var session = await _userRepository.GenerateSessionAsync(user);
            await _userRepository.AddSessionAsync(user, session);

            var model = _mapper.Map(user, _mapper.Map<UserSessionModel>(session));
            model.Roles = await _userRepository.GetRolesAsync(user);
            return model;
        }

        public async Task SignOutAsync(SignOutForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<SignOutForm.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(form);

            if (!formValidationResult.IsValid)
                throw new BadRequestException(formValidationResult.ToDictionary());

            var user = await _userRepository.FindByRefreshTokenAsync(form.RefreshToken);

            if (user == null) throw new BadRequestException(nameof(form.RefreshToken), $"'{nameof(form.RefreshToken).Humanize(LetterCasing.Title)}' is not valid.");

            await _userRepository.RemoveSessionAsync(user, form.RefreshToken);
        }
    }
}