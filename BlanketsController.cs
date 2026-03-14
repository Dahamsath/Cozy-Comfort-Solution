using CozyComfort.API.Data;
using CozyComfort.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CozyComfort.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlanketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BlanketsController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blanket>>> GetBlankets()
        {
            return await _context.Blankets.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Blanket>> GetBlanket(int id)
        {
            var blanket = await _context.Blankets.FindAsync(id);
            if (blanket == null) return NotFound();
            return blanket;
        }

        
        [HttpPost]
        public async Task<ActionResult<Blanket>> PostBlanket(Blanket blanket)
        {
            _context.Blankets.Add(blanket);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBlanket), new { id = blanket.Id }, blanket);
        }

        
        [HttpPut("{id}/production")]
        public async Task<IActionResult> UpdateProductionCapacity(int id, [FromBody] int newCapacity)
        {
            var blanket = await _context.Blankets.FindAsync(id);
            if (blanket == null) return NotFound();

            blanket.ProductionCapacity = newCapacity;
            

            await _context.SaveChangesAsync();
            return NoContent();
        }
       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlanket(int id, Blanket blanket)
        {
            if (id != blanket.Id)
            {
                return BadRequest("ID mismatch");
            }

            
            _context.Entry(blanket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Blankets.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); 
        }
    }
}