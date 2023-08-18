using AutoMapper;
using FluentValidation;
using Hubtel.eCommerce.Cart.Core.Constants;
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Exceptions;
using Hubtel.eCommerce.Cart.Core.Extensions.Identity;
using Hubtel.eCommerce.Cart.Core.Models.Accounts;
using Hubtel.eCommerce.Cart.Core.Models.Items;
using Hubtel.eCommerce.Cart.Core.Repositories;
using Hubtel.eCommerce.Cart.Core.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Services
{
    public class ItemService : IService
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _validatorProvider;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;

        public ItemService(IMapper mapper, IServiceProvider validatorProvider, IItemRepository itemRepository, IUserRepository userRepository, IUserContext userContext)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public async Task AddAsync(AddItemForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<AddItemForm.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(form);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            // Get the authorized user
            var currentUser = await _userRepository.GetUser(_userContext.User);
            if (currentUser == null) throw new UnauthorizedException();

            // Check if the authorized user is an admin
            var isCurrentUserAdmin = await _userRepository.IsInRoleAsync(currentUser, Roles.Admin);
            if (!isCurrentUserAdmin) throw new ForbiddenException();

            // Create a new item
            var item = _mapper.Map(form, new Item());
            item.CreatedAt = DateTimeOffset.UtcNow;
            item.UpdatedAt = DateTimeOffset.UtcNow;
            await _itemRepository.CreateAsync(item);
        }

        public async Task EditAsync(EditItemForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<AddItemForm.Validator>();
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

            // Edit the existing item
            item = _mapper.Map(form, new Item());
            item.UpdatedAt = DateTimeOffset.UtcNow;
            await _itemRepository.UpdateAsync(item);
        }

        public async Task DeleteAsync(DeleteItemForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<DeleteItemForm.Validator>();
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

        public async Task<GetItemModel> GetAsync(GetItemFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var formValidator = _validatorProvider.GetRequiredService<GetItemFilter.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(filter);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            var item = await _itemRepository.FindByIdAsync(filter.Id);
            if (item == null) throw new NotFoundException();

            // Get the existing item
            var itemModel = _mapper.Map(await _itemRepository.FindByIdAsync(filter.Id), new GetItemModel());
            return itemModel;

        }

        public async Task<IPageable<GetItemModel>> GetPageAsync(GetItemPageFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var formValidator = _validatorProvider.GetRequiredService<GetItemPageFilter.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(filter);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            var itemPage = await _itemRepository.FindManyAsync(filter.PageNumber, filter.PageSize, item =>
            {
                var itemModel = _mapper.Map(item, new GetItemModel());
                return itemModel;
            });
            return itemPage;
        }
    }
}