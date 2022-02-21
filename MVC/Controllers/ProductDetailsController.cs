using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers
{
    public class ProductDetailsController : Controller
    {
        private readonly ShopContext _context;

        public ProductDetailsController(ShopContext context)
        {
            _context = context;
        }

        // GET: ProductDetails
        public async Task<IActionResult> Index()
        {
            var shopContext = _context.ProductDetails.Include(p => p.Color).Include(p => p.Product).Include(p => p.Size);
            return View(await shopContext.ToListAsync());
        }

        // GET: ProductDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails
                .Include(p => p.Color)
                .Include(p => p.Product)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);
        }

        // GET: ProductDetails/Create
        public IActionResult Create()
        {
            ViewData["Color_id"] = new SelectList(_context.Set<Color>(), "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["Size_id"] = new SelectList(_context.Set<Size>(), "Id", "Id");
            return View();
        }

        // POST: ProductDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Size_id,Color_id,Quantity")] ProductDetail productDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Color_id"] = new SelectList(_context.Set<Color>(), "Id", "Id", productDetail.Color_id);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productDetail.ProductId);
            ViewData["Size_id"] = new SelectList(_context.Set<Size>(), "Id", "Id", productDetail.Size_id);
            return View(productDetail);
        }

        // GET: ProductDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails.FindAsync(id);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["Color_id"] = new SelectList(_context.Set<Color>(), "Id", "Id", productDetail.Color_id);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productDetail.ProductId);
            ViewData["Size_id"] = new SelectList(_context.Set<Size>(), "Id", "Id", productDetail.Size_id);
            return View(productDetail);
        }

        // POST: ProductDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Size_id,Color_id,Quantity")] ProductDetail productDetail)
        {
            if (id != productDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDetailExists(productDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Color_id"] = new SelectList(_context.Set<Color>(), "Id", "Id", productDetail.Color_id);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productDetail.ProductId);
            ViewData["Size_id"] = new SelectList(_context.Set<Size>(), "Id", "Id", productDetail.Size_id);
            return View(productDetail);
        }

        // GET: ProductDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails
                .Include(p => p.Color)
                .Include(p => p.Product)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);
        }

        // POST: ProductDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productDetail = await _context.ProductDetails.FindAsync(id);
            _context.ProductDetails.Remove(productDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductDetailExists(int id)
        {
            return _context.ProductDetails.Any(e => e.Id == id);
        }
    }
}
