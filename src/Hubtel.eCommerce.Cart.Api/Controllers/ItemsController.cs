﻿using Hubtel.eCommerce.Cart.Core.Models.Carts;
using Hubtel.eCommerce.Cart.Core.Models.Items;
using Hubtel.eCommerce.Cart.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class ItemsController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ItemsController(ItemService itemService)
        {
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        }

        [Authorize]
        [HttpGet("/[controller]/{id}")]
        public async Task<IActionResult> Get([FromRoute] long id)
        {
            return Ok(await _itemService.GetAsync(new GetItemFilter { Id = id }));
        }

        [Authorize]
        [HttpGet("/[controller]")]
        public async Task<IActionResult> GetPage([FromQuery] GetItemPageFilter filter)
        {
            return Ok(await _itemService.GetPageAsync(filter));
        }
    }
}