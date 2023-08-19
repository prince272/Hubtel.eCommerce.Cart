using Hubtel.eCommerce.Cart.Core.Models.Carts;
using Hubtel.eCommerce.Cart.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartsController(CartService cartService)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }

        [Authorize]
        [HttpPost("/[controller]/items")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartForm form)
        {
            await _cartService.AddItemAsync(form);
            return Ok();
        }

        [Authorize]
        [HttpDelete("/[controller]/items")]
        public async Task<IActionResult> RemoveItemFromCart([FromBody] RemoveItemFromCartForm form)
        {
            await _cartService.RemoveItemAsync(form);
            return Ok();
        }

        [HttpGet("/[controller]")]
        public async Task<IActionResult> GetPage([FromQuery] CartPageFilter filter)
        {
            return Ok(await _cartService.GetPageAsync(filter));
        }

        [HttpGet("/[controller]/active")]
        public async Task<IActionResult> GetActiveList([FromQuery] CartListFilter filter)
        {
            return Ok(await _cartService.GetActiveListAsync(filter));
        }
    }
}
