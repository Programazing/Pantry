using Microsoft.AspNetCore.Mvc;
using Pantry.Models;
using System;
using System.Threading.Tasks;

namespace Pantry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly AppDbContext Context;

        public AuthenticateController(AppDbContext context)
        {
            Context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(CurrentUser user)
        {
            if (!String.IsNullOrWhiteSpace(user.Username))
            {
                await Context.CurrentUser.AddAsync(new CurrentUser() { Username = user.Username });
                await Context.SaveChangesAsync();

                return Ok("Logged In!");

            }

            return NotFound("User Not Found!");
        }
    }
}
