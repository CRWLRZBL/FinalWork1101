using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalWeb_API.Data;
using FinalWeb_API.Models;

namespace FinalWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamManufacturersController : ControllerBase
    {
        private readonly ShopContext _context;

        public ExamManufacturersController(ShopContext context)
        {
            _context = context;
        }

        // GET: api/ExamManufacturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamManufacturer>>> GetExamManufacturers()
        {
            return await _context.ExamManufacturers.ToListAsync();
        }

        // GET: api/ExamManufacturers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamManufacturer>> GetExamManufacturer(int id)
        {
            var examManufacturer = await _context.ExamManufacturers.FindAsync(id);

            if (examManufacturer == null)
            {
                return NotFound();
            }

            return examManufacturer;
        }

        // PUT: api/ExamManufacturers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamManufacturer(int id, ExamManufacturer examManufacturer)
        {
            if (id != examManufacturer.ManufacturerId)
            {
                return BadRequest();
            }

            _context.Entry(examManufacturer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamManufacturerExists(id))
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

        // POST: api/ExamManufacturers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExamManufacturer>> PostExamManufacturer(ExamManufacturer examManufacturer)
        {
            _context.ExamManufacturers.Add(examManufacturer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExamManufacturer", new { id = examManufacturer.ManufacturerId }, examManufacturer);
        }

        // DELETE: api/ExamManufacturers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamManufacturer(int id)
        {
            var examManufacturer = await _context.ExamManufacturers.FindAsync(id);
            if (examManufacturer == null)
            {
                return NotFound();
            }

            _context.ExamManufacturers.Remove(examManufacturer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExamManufacturerExists(int id)
        {
            return _context.ExamManufacturers.Any(e => e.ManufacturerId == id);
        }
    }
}
