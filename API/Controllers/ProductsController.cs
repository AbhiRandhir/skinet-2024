using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IUnitOfWork unit) : BaseApiController
    {   
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)     
        {
            //return await context.Products.ToListAsync(); //This methods comes from normal Products without interface
            
            //return Ok(await repo.GetProductAsync(brand, type, sort)); //Without generic repo

            //Start - Specification Pattern
                var spec = new ProductSpecification(specParams);

                return await CreatePageResult(unit.Repository<Product>(), spec, specParams.PageIndex, specParams.PageSize);    
            //End - Specification Pattern
        }

        [HttpGet("{id:int}")] //api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
           // var product = await context.Products.FindAsync(id);

           var product = await unit.Repository<Product>().GetByIdAsync(id);

            if(product == null)
                return NotFound();

            return product;    
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            //context.Products.Add(product);

            //await context.SaveChangesAsync();

            unit.Repository<Product>().Add(product);

            if(await unit.Complete()){
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

            unit.Repository<Product>().Update(product);

            if(await unit.Complete()){
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

            var product = await unit.Repository<Product>().GetByIdAsync(id);

            if(product == null)
                return NotFound();

            unit.Repository<Product>().Remove(product);

               if(await unit.Complete()){
                return NoContent();
            } 

            return BadRequest("Problem deletion the product");  

        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            // TODO: Implement method
                //Start - Speification Pattern
                    var spec = new BrandListSpecification();
                //End - Speification Pattern

            return Ok(await unit.Repository<Product>().ListAsync(spec));
            //return Ok(await repo.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            // TODO: Implement method
               //Start - Speification Pattern
                    var spec = new TypeListSpecification();
                //End - Speification Pattern
            return Ok(await unit.Repository<Product>().ListAsync(spec));
            //return Ok(await repo.GetTypesAsync());
        }
        

        private bool ProductExists(int id)
        {
            //return context.Products.Any(x => x.Id == id);

            return unit.Repository<Product>().Exists(id);
        }
    }
}
