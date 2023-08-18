using Hubtel.eCommerce.Cart.Core.Models.Accounts;
using Hubtel.eCommerce.Cart.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountsController(AccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInForm form)
        {
            return Ok(await _accountService.SignInAsync(form));
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpForm form)
        {
            await _accountService.SignUpAsync(form);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RefreshSession(RefreshSessionForm form)
        {
            return Ok(await _accountService.RefreshSessionAsync(form));
        }

        [HttpPost]
        public async Task<IActionResult> SignOut(SignOutForm form)
        {
            await _accountService.SignOutAsync(form);
            return Ok();
        }
    }
}