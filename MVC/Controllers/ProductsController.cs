using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using PagedList;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace MVC.Controllers
{

    public class ProductsController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        
        // GET: Products
        [HttpGet]
        public async Task<IActionResult> Index(string sku, string name, int? minprice, int? maxprice, int categoryID = 0)
        {
            List<Product> products = _context.Products.ToList();
            List<ProductType> lst = _context.ProductTypes.ToList();
            lst.Insert(0, new ProductType { Id = 0, Name = "- - - Tất Cả - - -" });
            SelectList list = new SelectList(lst, "Id", "Name");
            ViewBag.ProductType = list;
            if (categoryID != 0)
            {
                products = products.Where(prd => prd.ProductTypeId == categoryID).ToList();
            }   
            if (sku != null)
            {
                products = products.Where(prd => prd.SKU == sku).ToList();
            }
            if (name != null)
            {
                products = products.Where(pro => pro.Name == name).ToList();
            }
            if (minprice == null)
            {
                minprice = 0;
            }
            if (maxprice == null)
            {
                maxprice = int.MaxValue;
            }

            products = products.Where(prd => prd.Price >= minprice && prd.Price <= maxprice).ToList();
           
            return View(products);

        }
        
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType).Include(d => d.ProductDetails)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,Image,ImageFile,Status")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                if (product.ImageFile != null)
                {
                    //Xử lí
                    var filename = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "product");
                    var filePath = Path.Combine(uploadPath, filename);
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        product.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    product.Image = filename;
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,Image,Status")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ByPriceRange()
        {
            var sp = _context.Products.Where(sp => sp.Price >= 40000 && sp.Price <= 60000).ToList();
            return View(sp);
        }
        public IActionResult ByProductType()
        {
            var sp = from prd in _context.Products
                     join prdt in _context.ProductTypes
                     on prd.ProductTypeId equals prdt.Id
                     where prdt.Name == "Áo công sở"
                     select  prd;
            return View(sp);
        }
        public IActionResult ByStock()
        {
            var sp = _context.Products.Where(sp => sp.Stock <10).ToList();
            return View(sp);
        }
        public IActionResult Search(string sku, string name, int? minprice, int? maxprice, int categoryID = 0)
        {

            List<Product> products = _context.Products.ToList();
            List<ProductType> lst = _context.ProductTypes.ToList();
            lst.Insert(0, new ProductType { Id = 0, Name = "- - - Tất Cả - - -" });
            SelectList list = new SelectList(lst, "Id", "Name");
            ViewBag.ProductType = list;
            if (categoryID != 0)
            {
                products = products.Where(prd => prd.ProductTypeId == categoryID).ToList();
            }
            if (sku != null)
            {
                products = products.Where(prd => prd.SKU == sku).ToList();
            }
            if (name != null)
            {
                products = products.Where(pro => pro.Name == name).ToList();
            }
            if (minprice == null)
            {
                minprice = 0;
            }
            if (maxprice == null)
            {
                maxprice = int.MaxValue;
            }

            products = products.Where(prd => prd.Price >= minprice && prd.Price <= maxprice).ToList();

            return View(products);
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }

    internal class HandleErrorAttribute : Attribute
    {
    }
}
