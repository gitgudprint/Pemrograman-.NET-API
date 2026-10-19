using IF133.Tugas1.Api.Data;
using IF133.Tugas1.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IF133.Tugas1.Api.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsController(AppDbContext db) : ControllerBase
{
    // GET /api/products
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await db.Products.AsNoTracking().ToListAsync();
        return Ok(products);
    }

    // GET /api/products/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = $"Product dengan id {id} tidak ditemukan." });

        return Ok(product);
    }

    // POST /api/products
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        product.Id = 0; // Id dibuat otomatis oleh database
        db.Products.Add(product);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT /api/products/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> Update(int id, Product input)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = $"Product dengan id {id} tidak ditemukan." });

        product.Name = input.Name;
        product.Description = input.Description;
        product.Price = input.Price;
        product.Stock = input.Stock;
        await db.SaveChangesAsync();

        return Ok(product);
    }

    // DELETE /api/products/{id}
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = $"Product dengan id {id} tidak ditemukan." });

        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
