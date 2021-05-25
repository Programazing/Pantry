using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pantry.Models;
using Splitio.Services.Client.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pantry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PantryController : Controller
    {
        private readonly AppDbContext Context;
        public ISplitClient Client { get; }
        private bool ShowImageLocation
        {
            get { return GetStateOfImageLocation(); }
        }


        public PantryController(AppDbContext context, ISplitFactory split)
        {
            Context = context;

            var client = split.Client();
            client.BlockUntilReady(10000);

            Client = client;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() => await Context.Products.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id) => await Context.Products.FindAsync(id) ?? (ActionResult<Product>)NotFound();

        [HttpPost]
        public async Task<ActionResult<int>> PostProduct(Product product)
        {
            var entityProduct = await Context.Products.AddAsync(product);
            await Context.SaveChangesAsync();

            return entityProduct.Entity.Id;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await Context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            Context.Products.Remove(product);
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("/api/[controller]/image")]
        public async Task<ActionResult<ImageLocation>> GetProductImages(int id)
        {
            if (CurrentUserIsAuthorized())
            {
                if (ShowImageLocation is false)
                {
                    return NotFound();
                }

                var output = await Context.ImageLocations.FindAsync(id);

                return output; 
            }

            return Unauthorized();
        }

        [HttpGet("/api/[controller]/image/{id}")]
        public async Task<ActionResult<ImageLocation>> GetProductImage(int id)
        {
            if (CurrentUserIsAuthorized())
            {
                if (ShowImageLocation is false)
                {
                    return NotFound();
                }

                return await Context.ImageLocations.FindAsync(id); 
            }

            return Unauthorized();
        }

        [HttpPost("/api/[controller]/image")]
        public async Task<ActionResult<int>> PostProductImage(ImageLocation imageLocation)
        {
            if (CurrentUserIsAuthorized())
            {
                if (ShowImageLocation is false)
                {
                    return NotFound();
                }

                var entityProduct = await Context.ImageLocations.AddAsync(imageLocation);
                await Context.SaveChangesAsync();

                return entityProduct.Entity.Id; 
            }

            return Unauthorized();
        }

        private bool GetStateOfImageLocation()
        {
            var treatment = Client.GetTreatment("Default_Value", "Pantry_API_ImageLocation");

            if (treatment == "on")
            {
                return true;
            }

            if(treatment == "off")
            {
                return false;
            }

            throw new System.Exception("Something went wrong!");
        }

        private bool CurrentUserIsAuthorized()
        {
            var currentUser = Context.CurrentUser.FirstOrDefault();
            if (currentUser != null)
            {
                var treatment = Client.GetTreatment(currentUser.Username, "Pantry_API_Entitlements");

                if (treatment == "on")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
