using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalWeb_API.Data;
using FinalWeb_API.Models;
using ServicesLibrary.DTOs;

namespace FinalWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamOrdersController : ControllerBase
    {
        private readonly ShopContext _context;

        public ExamOrdersController(ShopContext context)
        {
            _context = context;
        }

        // GET: api/ExamOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamOrder>>> GetExamOrders()
        {
            return await _context.ExamOrders.ToListAsync();
        }

        // GET: api/ExamOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamOrder>> GetExamOrder(int id)
        {
            var examOrder = await _context.ExamOrders.FindAsync(id);

            if (examOrder == null)
            {
                return NotFound();
            }

            return examOrder;
        }

        // PUT: api/ExamOrders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamOrder(int id, ExamOrder examOrder)
        {
            if (id != examOrder.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(examOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamOrderExists(id))
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

        // POST: api/ExamOrders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExamOrder>> PostExamOrder(ExamOrder examOrder)
        {
            _context.ExamOrders.Add(examOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExamOrder", new { id = examOrder.OrderId }, examOrder);
        }

        // DELETE: api/ExamOrders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamOrder(int id)
        {
            var examOrder = await _context.ExamOrders.FindAsync(id);
            if (examOrder == null)
            {
                return NotFound();
            }

            _context.ExamOrders.Remove(examOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamOrderExists(int id)
        {
            return _context.ExamOrders.Any(e => e.OrderId == id);
        }

        // GET: api/ExamOrders/TotalCost
        [HttpGet("TotalCost")]
        public async Task<ActionResult<List<OrderSummaryDTO>>> GetOrdersAsync()
        {
            try
            {
                var orders = await _context.ExamOrders.Include(p => p.ExamOrderProducts).ThenInclude(op => op.ProductArticleNumberNavigation)
                .Select(g => new OrderSummaryDTO
                {
                    OrderID = g.OrderId,
                    UserID = g.UserId,
                    OrderStatus = g.OrderStatus,
                    OrderDate = g.OrderDate,
                    OrderDeliveryDate = g.OrderDeliveryDate,
                    OrderPickupPoint = g.OrderPickupPoint,
                    OrderPickupCode = g.OrderPickupCode,
                    TotalCost = g.ExamOrderProducts
                        .Sum(op => op.ProductArticleNumberNavigation.ProductCost * (100 - op.ProductArticleNumberNavigation.ProductDiscountAmount) / 100 * op.Amount)
                }).ToListAsync();

                if (orders == null)
                {
                    return NotFound();
                }
                return orders;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Inneral server error: " + ex.Message);
            }
        }

        // GET: api/ExamOrders/5/TotalCost
        [HttpGet("{id}/TotalCost")]
        public async Task<ActionResult<OrderSummaryDTO>> GetOrderByIdWithTotalCostAsync(int id)
        {
            try
            {
                var order = await _context.ExamOrders.Include(p => p.ExamOrderProducts).ThenInclude(op => op.ProductArticleNumberNavigation)
                .Select(g => new OrderSummaryDTO
                {
                    OrderID = g.OrderId,
                    UserID = g.UserId,
                    OrderStatus = g.OrderStatus,
                    OrderDate = g.OrderDate,
                    OrderDeliveryDate = g.OrderDeliveryDate,
                    OrderPickupPoint = g.OrderPickupPoint,
                    OrderPickupCode = g.OrderPickupCode,
                    TotalCost = g.ExamOrderProducts
                        .Sum(op => op.ProductArticleNumberNavigation.ProductCost * (100 - op.ProductArticleNumberNavigation.ProductDiscountAmount) / 100 * op.Amount)
                }).FirstOrDefaultAsync(g => g.OrderID == id);

                if (order == null)
                {
                    return NotFound();
                }
                return order;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Inneral server error: " + ex.Message);
            }
        }
    }
}
