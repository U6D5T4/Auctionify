using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
        private readonly IIdentityService identityService;
        private readonly IConfiguration configuration;

        public AuthController(IIdentityService identityService, IConfiguration configuration)
        {
            this.identityService = identityService;
            this.configuration = configuration;
        }

        // api/auth/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await identityService.RegisterUserAsync(model);

                if (result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid"); // statusCode: 400 (something wrong with client request)
        }

        // api/auth/confirmemail?userid&token
        [HttpGet]
        [Route("confirmemail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await identityService.ConfirmUserEmailAsync(userId, token);

            if (result.IsSuccess)
                return Redirect($"{configuration["AppUrl"]}/confirmemail.html");

            return BadRequest(result);
        }
    }
}
