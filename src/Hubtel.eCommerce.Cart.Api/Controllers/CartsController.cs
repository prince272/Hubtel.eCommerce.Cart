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
        [HttpPost("/[controller]/[action]")]
        public async Task<IActionResult> Process([FromBody] ProcessCartForm form)
        {
            await _cartService.ProcessAsync(form);
            return Ok();
        }

        [Authorize]
        [HttpDelete("/[controller]/{id}")]
        public async Task<IActionResult> Delete([FromRoute] long id, [FromBody] DeleteCartForm form)
        {
            if (form != null) form.Id = id;
            await _cartService.DeleteAsync(form);
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
