using AutoMapper;
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
using System.Linq.Expressions;
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

        public async Task AddItemAsync(AddItemToCartForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            var formValidator = _validatorProvider.GetRequiredService<AddItemToCartForm.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(form);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            await ProcessAsync(form.ItemId, form.Quantity);
        }

        public async Task RemoveItemAsync(RemoveItemFromCartForm form)
        {
            await ProcessAsync(form.ItemId, 0);
        }

        public async Task ProcessAsync(long itemId, int quantity)
        {

            const int cartLimit = 100;

            // Get the authorized user
            var currentUser = await _userRepository.GetUser(_userContext.User) ?? throw new UnauthorizedException();
            var item = await _itemRepository.FindByIdAsync(itemId) ?? throw new BadRequestException($"Item '{itemId}' does not exist.");

            var carts = (await _cartRepository.FindManyAsync(cart => cart.UserId == currentUser.Id)).ToArray();
            var activeCart = carts.FirstOrDefault(cart => cart.ItemId == item.Id);

            if (quantity == 0)
            {
                if (activeCart != null)
                {
                    await _cartRepository.DeleteAsync(activeCart);
                    // Item has removed from the cart.
                }
            }
            else
            {
                if (activeCart == null)
                {
                    if (carts.Length >= cartLimit)
                    {
                        throw new BadRequestException($"Cart '{item.Id}' limit has been reached.");
                    }

                    activeCart = new Entities.Cart();
                    activeCart.UserId = currentUser.Id;
                    activeCart.ItemId = item.Id;
                    activeCart.Quantity = quantity;
                    activeCart.CreatedAt = DateTimeOffset.UtcNow;
                    activeCart.UpdatedAt = DateTimeOffset.UtcNow;
                    await _cartRepository.CreateAsync(activeCart);

                    // Item has been added to cart.
                }
                else
                {
                    activeCart.UserId = currentUser.Id;
                    activeCart.ItemId = item.Id;
                    activeCart.Quantity = quantity;
                    activeCart.UpdatedAt = DateTimeOffset.UtcNow;
                    await _cartRepository.UpdateAsync(activeCart);

                    // Item has been updated to cart.
                }
            }

        }

        public async Task<CartListModel> GetActiveListAsync(CartListFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var formValidator = _validatorProvider.GetRequiredService<CartListFilter.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(filter);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            var predicate = PredicateBuilder.True<Entities.Cart>();

            var currentUser = await _userRepository.GetUser(_userContext.User) ?? throw new UnauthorizedException();

            if (!(await _userRepository.IsInRoleAsync(currentUser, Roles.Admin)))
                predicate = predicate.And(cart => cart.UserId == currentUser.Id);

             predicate = predicate.And(await GetPredicateAsync(filter));

            var carts = await _cartRepository.FindManyAsync(predicate, include: cart => cart.Item);
            var cartListModel = MapCartListModel<CartListModel>(carts);
            return cartListModel;
        }

        public async Task<CartPageModel> GetPageAsync(CartPageFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var formValidator = _validatorProvider.GetRequiredService<CartPageFilter.Validator>();
            var formValidationResult = await formValidator.ValidateAsync(filter);
            if (!formValidationResult.IsValid) throw new BadRequestException(formValidationResult.ToDictionary());

            var predicate = PredicateBuilder.True<Entities.Cart>();

            predicate = predicate.And(await GetPredicateAsync(filter));

            
            var cartPageModel = MapCartPageModel(await _cartRepository.FindManyAsync(filter.PageNumber, filter.PageSize, predicate, orderBy: null, include: cart => cart.Item));
            return cartPageModel;
        }

        private Task<Expression<Func<Entities.Cart, bool>>> GetPredicateAsync<TFilter>(TFilter filter)
            where TFilter : CartListFilter
        {
            var predicate = PredicateBuilder.True<Entities.Cart>();

            if (filter.Ids != null && filter.Ids.Any())
                predicate = predicate.And(cart => filter.Ids.Contains(cart.Id));

            if (filter.ItemIds != null && filter.ItemIds.Any())
                predicate = predicate.And(cart => filter.ItemIds.Contains(cart.ItemId));

            if (filter.Quantity != null)
                predicate = predicate.And(cart => filter.Quantity == cart.Quantity);

            if (filter.Time != null)
            {
                // Calculate the time duration before the current UTC time.
                var before = DateTimeOffset.UtcNow - filter.Time;

                // Checks if the 'before' time duration is less than or equal to the 'UpdatedAt' time of a cart.
                predicate = predicate.And(cart => before <= cart.UpdatedAt);
            }


            if (filter.PhoneNumbers != null && filter.PhoneNumbers.Any())
            {
                predicate = predicate.And(cart => filter.PhoneNumbers.Contains(cart.User != null ? cart.User.PhoneNumber : null));
            }

            return Task.FromResult(predicate);
        }

        private TListModel MapCartListModel<TListModel>(IEnumerable<Entities.Cart> carts)
            where TListModel : CartListModel
        {

            var cartModels = new List<CartModel>();

            foreach (var cart in carts)
            {
                var cartModel = _mapper.Map(cart, new CartModel());
                cartModel.ItemName = cart.Item.Name;
                cartModel.UnitPrice = cart.Item.Price;
                cartModel.Amount = cart.Item.Price * cart.Quantity;
                cartModels.Add(cartModel);
            }

            var cartListModel = Activator.CreateInstance<TListModel>();
            cartListModel.Items = cartModels.ToArray();
            cartListModel.TotalAmount = carts.Select(cart => (cart.Item.Price * cart.Quantity)).Sum();

            return cartListModel;
        }

        private CartPageModel MapCartPageModel(IPageable<Entities.Cart> cartPage)
        {
            var model = MapCartListModel<CartPageModel>(cartPage.Items);
            model.PageNumber = cartPage.PageNumber;
            model.PageSize = cartPage.PageSize;
            model.TotalItems = cartPage.TotalItems;
            model.TotalPages = cartPage.TotalPages;
            return model;
        }
    }
}