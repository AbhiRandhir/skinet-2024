using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository repo) : ControllerBase
    {
    
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)     
        {
            //return await context.Products.ToListAsync(); //This methods comes from normal Products without interface
            
            return Ok(await repo.GetProductAsync(brand, type, sort));
        }

        [HttpGet("{id:int}")] //api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
           // var product = await context.Products.FindAsync(id);

           var product = await repo.GetProductByIdAsync(id);

            if(product == null)
                return NotFound();

            return product;    
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            //context.Products.Add(product);

            //await context.SaveChangesAsync();

            repo.AddProduct(product);

            if(await repo.SaveChangesAsync()){
                return CreatedAtAction("GetProduct",new {id = product.Id}, product);
            }

            return BadRequest("Problem creating product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id,Product product)
        {
            // if(product.Id != id || !ProductExists(id))
            //     return BadRequest("Cannot update this product");

            // context.Entry(product).State = EntityState.Modified;

            // await context.SaveChangesAsync();

            // return NoContent();

             if(product.Id != id || !ProductExists(id))
                 return BadRequest("Cannot update this product");

            repo.UpdateProduct(product);

            if(await repo.SaveChangesAsync()){
                return NoContent();
            }     
            return BadRequest("Problem updating the product");

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            // var product = await context.Products.FindAsync(id);

            // if(product == null)
            //     return NotFound();

            // context.Products.Remove(product);

            // await context.SaveChangesAsync();

            // return NoContent(); 

            var product = await repo.GetProductByIdAsync(id);

            if(product == null)
                return NotFound();

            repo.DeleteProduct(product);

               if(await repo.SaveChangesAsync()){
                return NoContent();
            } 

            return BadRequest("Problem deletion the product");  

        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repo.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repo.GetTypesAsync());
        }
        

        private bool ProductExists(int id)
        {
            //return context.Products.Any(x => x.Id == id);

            return repo.ProductExists(id);
        }
    }
}
