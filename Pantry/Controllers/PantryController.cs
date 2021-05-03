using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pantry.Models;
using Splitio.Services.Client.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pantry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PantryController : Controller
    {
        private readonly AppDbContext _context;
        private bool ShowImageLocation { get; }

        public PantryController(AppDbContext context, ISplitFactory split)
        {
            _context = context;
            var client = split.Client();
            client.BlockUntilReady(10000);

            var treatment = client.GetTreatment("Default_Value", "Pantry_API_ImageLocation");

            if(treatment == "on")
            {
                ShowImageLocation = true;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() => await _context.Products.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id) => await _context.Products.FindAsync(id) ?? (ActionResult<Product>)NotFound();

        [HttpPost]
        public async Task<ActionResult<int>> PostProduct(Product product)
        {
            var entityProduct = await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return entityProduct.Entity.Id;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("/api/[controller]/image")]
        public async Task<ActionResult<ImageLocation>> GetProductImages(int id)
        {
            if (ShowImageLocation is false)
            {
                return NotFound();
            }

            var output = await _context.ImageLocations.FindAsync(id);

            return output;
        }

        [HttpGet("/api/[controller]/image/{id}")]
        public async Task<ActionResult<ImageLocation>> GetProductImage(int id) => await _context.ImageLocations.FindAsync(id) ?? (ActionResult<ImageLocation>)NotFound();

        [HttpPost("/api/[controller]/image")]
        public async Task<ActionResult<int>> PostProductImage(ImageLocation imageLocation)
        {
            var entityProduct = await _context.ImageLocations.AddAsync(imageLocation);
            await _context.SaveChangesAsync();

            return entityProduct.Entity.Id;
        }
    }
}
