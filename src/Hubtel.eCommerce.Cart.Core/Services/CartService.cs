﻿using AutoMapper;
using Hubtel.eCommerce.Cart.Core.Constants;
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Exceptions;
using Hubtel.eCommerce.Cart.Core.Extensions.Identity;
using Hubtel.eCommerce.Cart.Core.Models.Carts;
using Hubtel.eCommerce.Cart.Core.Models.Items;
using Hubtel.eCommerce.Cart.Core.Repositories;
using Hubtel.eCommerce.Cart.Core.Shared;
using Hubtel.eCommerce.Cart.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Services
{
    public class CartService : IService
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _validatorProvider;
        private readonly ICartRepository _cartRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;

        public CartService(IMapper mapper, IServiceProvider validatorProvider, ICartRepository cartRepository, IItemRepository itemRepository, IUserRepository userRepository, IUserContext userContext)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public async Task ProcessAsync(ProcessCartForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<ProcessCartForm.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(form);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            const int cartLimit = 11;
            const int itemMaxQuantity = 10;

            // Get the authorized user
            var currentUser = await _userRepository.GetUser(_userContext.User) ?? throw new UnauthorizedException();
            var item = await _itemRepository.FindByIdAsync(form.ItemId) ?? throw new BadRequestException($"Item '{form.ItemId}' does not exist.");

            var carts = (await _cartRepository.FindManyAsync(cart => cart.UserId == currentUser.Id)).ToArray();
            var activeCart = carts.FirstOrDefault(cart => cart.ItemId == item.Id);

            if (form.Quantity == 0)
            {
                if (activeCart != null)
                {
                    await _cartRepository.DeleteAsync(activeCart);
                    // Item has removed from the cart.
                }
            }
            else
            {
                if (form.Quantity >= itemMaxQuantity)
                {
                    throw new BadRequestException($"Item '{item.Id}' limit has been reached.");
                }

                if (activeCart == null)
                {
                    if (carts.Length >= cartLimit)
                    {
                        throw new BadRequestException($"Cart '{item.Id}' limit has been reached.");
                    }

                    activeCart = new Entities.Cart();
                    activeCart.UserId = currentUser.Id;
                    activeCart.ItemId = item.Id;
                    activeCart.ItemName = item.Name;
                    activeCart.Quantity = form.Quantity;
                    activeCart.CreatedAt = DateTimeOffset.UtcNow;
                    activeCart.UpdatedAt = DateTimeOffset.UtcNow;
                    await _cartRepository.CreateAsync(activeCart);

                    // Item has been added to cart.
                }
                else
                {
                    activeCart.UserId = currentUser.Id;
                    activeCart.ItemId = item.Id;
                    activeCart.ItemName = item.Name;
                    activeCart.Quantity = form.Quantity;
                    activeCart.UpdatedAt = DateTimeOffset.UtcNow;
                    await _cartRepository.UpdateAsync(activeCart);

                    // Item has been updated to cart.
                }
            }

        }

        public async Task DeleteAsync(DeleteCartForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<DeleteCartForm.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(form);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            // Get the authorized user
            var currentUser = await _userRepository.GetUser(_userContext.User);
            if (currentUser == null) throw new UnauthorizedException();

            // Check if the authorized user is an admin
            var isCurrentUserAdmin = await _userRepository.IsInRoleAsync(currentUser, Roles.Admin);
            if (!isCurrentUserAdmin) throw new ForbiddenException();

            var item = await _itemRepository.FindByIdAsync(form.Id);
            if (item == null) throw new NotFoundException();

            // Delete the existing item by marking the item as deleted
            await _itemRepository.DeleteAsync(item);
        }

        public async Task<IPageable<GetCartModel>> GetPageAsync(GetCartPageFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var formValidator = _validatorProvider.GetRequiredService<GetCartPageFilter.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(filter);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            var predicate = PredicateBuilder.True<Entities.Cart>();

            if (filter.Ids != null && filter.Ids.Any())
                predicate = predicate.And(cart => filter.Ids.Contains(cart.Id));

            if (filter.ItemIds != null && filter.ItemIds.Any())
                predicate = predicate.And(cart => filter.ItemIds.Contains(cart.ItemId));

            if (filter.Quantity != null)
                predicate = predicate.And(cart => filter.Quantity == cart.Quantity);

            var current = DateTimeOffset.UtcNow;

            if (filter.Time != null)
                predicate = predicate.And(cart => (current - cart.UpdatedAt) < filter.Time);

            if (filter.PhoneNumbers != null && filter.PhoneNumbers.Any())
                predicate = predicate.And(cart => filter.PhoneNumbers.Contains(cart.User != null ? cart.User.PhoneNumber : null));

            var cartPage = await _cartRepository.FindManyAsync(filter.PageNumber, filter.PageSize, item =>
            {
                var itemModel = _mapper.Map(item, new GetCartModel());
                return itemModel;
            }, predicate);
            return cartPage;
        }
    }
}