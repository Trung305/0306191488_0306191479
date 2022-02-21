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
    public class InvoicesController : Controller
    {
        private readonly ShopContext _context;

        public InvoicesController(ShopContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var shopContext = _context.Invoices.Include(i => i.Account);
            return View(await shopContext.ToListAsync());
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username");
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,AccountId,IssuedDate,ShippingAddress,ShippingPhone,Total,Status")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", invoice.AccountId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ByTotalRange()
        {
            var hd = _context.Invoices.Where(hd => hd.Total >= 400000 && hd.Total <= 600000).ToList();
            return View(hd);
        }
        public IActionResult ByAccountName()
        {
            var hd = from inv in _context.Invoices
                     join acc in _context.Accounts
                     on inv.AccountId equals acc.Id
                     where acc.Username.Contains("nam123")
                     select inv;
            return View(hd);
        }
        public IActionResult ByAccountAddress()
        {
            //var hd = _context.Invoices.Join(_context.Accounts, inv => inv.AccountId, acc => acc.Id, (inv, acc) => new { Name = inv.Total });
            //var hd = _context.Invoices.Where(inv => inv.Account.Address.Contains("@Tp.Hồ Chí Minh")).Select(inv => new {inv ,TypeName = inv.Account.Username});
            var hd = from inv in _context.Invoices
                     join acc in _context.Accounts
                     on inv.AccountId equals acc.Id
                     where acc.Address.Contains("Tp.Hồ Chí Minh")
                     select inv;
            return View(hd);
            
        }
        public IActionResult ByProduct()
        {
            var hd = from inv in _context.Invoices
                     join ct_inv in _context.InvoiceDetails on inv.Id equals ct_inv.InvoiceId
                     join prd in _context.Products on ct_inv.ProductId equals prd.Id
                     where prd.SKU == "WT3WPGZ9BTWB"
                     select inv;
            return View(hd);
        }
        public IActionResult ByProductType()
        {
            var hd = from inv in _context.Invoices
                     join ct_inv in _context.InvoiceDetails on inv.Id equals ct_inv.InvoiceId
                     join prd in _context.Products on ct_inv.ProductId equals prd.Id
                     where prd.ProductType.Name.Contains("Áo dài")|| prd.ProductType.Name.Contains("Áo công sở")
                     select inv;
            return View(hd);
        }
        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
    }
}
