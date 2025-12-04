using Microsoft.AspNetCore.Mvc;
using DotNetSqlCRUDApp.Models;
using DotNetSqlCRUDApp.Data;
using Microsoft.EntityFrameworkCore;

namespace DotNetSqlCRUDApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context) { _context = context; }
        
        public async Task<IActionResult> Index() => View(await _context.Products.ToListAsync());
        public IActionResult Create() => View();
        
        [HttpPost] public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid) { _context.Add(product); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(product);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            return product == null ? NotFound() : View(product);
        }
        
        [HttpPost] public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            if (ModelState.IsValid) { _context.Update(product); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(product);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            return product == null ? NotFound() : View(product);
        }
        
        [HttpPost, ActionName("Delete")] public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null) { _context.Products.Remove(product); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}
