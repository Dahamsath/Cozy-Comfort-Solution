using CozyComfort.API.Data;
using CozyComfort.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CozyComfort.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return order;
        }

        
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            order.Status = "Pending"; 
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            
            var blanket = await _context.Blankets.FindAsync(order.BlanketId);
            if (blanket == null) return NotFound("Blanket not found");

            
            if (blanket.StockLevel < order.Quantity)
            {
                return BadRequest("Not enough stock to fulfill this order.");
            }

           
            order.AssignedDistributor = "Central Distributor (Auto-Assigned)";

            
            order.ProcessedDate = DateTime.Now;

            
            blanket.StockLevel -= order.Quantity;

            
            order.Status = "Approved";

            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpPut("{id}/ship")]
        public async Task<IActionResult> ShipOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = "Shipped";
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}