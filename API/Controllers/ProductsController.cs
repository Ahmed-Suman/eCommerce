using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IGenericRepository<Product> productRepo) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            var spec = new ProductSpecification(brand,type,sort);
            var products = await productRepo.ListAsync(spec);
            return Ok(products);
        }

        [HttpGet("{id:int}")] //api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await productRepo.GetByIdAsync(id);
            if(product == null) return NotFound();
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
           productRepo.Add(product);
           if(await productRepo.SaveAllAsync())
           {
                return CreatedAtAction("GetProduct",new {id = product.Id}, product);
           }
          
           return BadRequest("Problem creating product");
        }

        [HttpPut("id:int")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if(product.Id != id || !ProductExists(id))
                return BadRequest("Cannot update this product");

            productRepo.Update(product);
             if(await productRepo.SaveAllAsync())
             {
                 return NoContent();
             }
           return BadRequest("Problem updating the product");
        }

        [HttpDelete("id:int")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await productRepo.GetByIdAsync(id);
            if(product == null) return NotFound();

            productRepo.Remove(product);
             if(await productRepo.SaveAllAsync())
             {
                 return NoContent();
             }
           return BadRequest("Problem deleting the product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec = new BrandListSpecification();
            return Ok(await productRepo.ListAsync(spec));
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();
            return Ok(await productRepo.ListAsync(spec));
        }
        private bool ProductExists(int id)
        {
            return productRepo.Exist(id);
        }
    }
}
