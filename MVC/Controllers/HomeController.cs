using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Data;
using MVC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ShopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //Hiển thị Username từ cookie
            /*if(HttpContext.Request.Cookies.ContainsKey("AccountUsername"))
            {
                ViewBag.AccountUsername = HttpContext.Request.Cookies["AccountUsername"].ToString();
            } */
            //Session
            if (HttpContext.Session.Keys.Contains("AccountUsername"))
            {
                ViewBag.AccountUsername = HttpContext.Session.GetString("AccountUsername");
            }
            var prd = _context.Products.ToList();
            return View(prd);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/Product/{productid:int}", Name = "Product")]
        public IActionResult Product([FromRoute] int productid)
        {
            var prd = _context.Products.Where(prd => prd.Id == productid).ToList();
            return View(prd);
        }
        public IActionResult Productlist()
        {
            List<Product> products = _context.Products.ToList();
            products = products.ToList();
            //var prd = _context.Products.ToList();
            return View(products);
        }
        public IActionResult ProductBC()
        {
            var products = _context.Products.Include(p => p.InvoiceDetails).ToList();
            //var prd = _context.Products.ToList();
            return View(products);
        }
        [Route("register")]
        public IActionResult Register()
        {

            return View();
        }
        public async Task<IActionResult> Cart()
        {
            string username = HttpContext.Session.GetString("AccountUsername");
            var carts = _context.Carts.Include(c => c.Account).Include(c => c.Product)
                                      .Where(c => c.Account.Username == username);
            ViewBag.Total = carts.Sum(c => c.Quantity * c.Product.Price);
            return View(await carts.ToListAsync());
        }
        public IActionResult Addcart(int id)
        {
            return Addcart(id, 1);
        }
        [HttpPost]
        public IActionResult Addcart(int productId, int quantity)
        {
            if (HttpContext.Session.GetString("AccountUsername") != null)
            {
                string username = HttpContext.Session.GetString("AccountUsername");
                int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
                Cart cart = _context.Carts.FirstOrDefault(c => c.AccountId == accountId && c.ProductId == productId);
                if (cart == null)
                {
                    cart = new Cart();
                    cart.AccountId = accountId;
                    cart.ProductId = productId;
                    cart.Quantity = quantity;
                    _context.Carts.Add(cart);
                }
                else
                {
                    cart.Quantity += quantity;
                }
                _context.SaveChanges();
                return RedirectToAction("Cart");
            }
            else
            {
                return RedirectToAction("Login2", "Accounts");
            }

        }
        /// xóa item trong cart
        [Route("/RemoveCart/{productid:int}", Name = "RemoveCart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            string username = HttpContext.Session.GetString("AccountUsername");
            int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
            Cart cart = _context.Carts.FirstOrDefault(c => c.AccountId == accountId && c.ProductId == productid);
            var cartitem = _context.Carts.Where(p => p.Product.Id == productid);
            foreach (Cart c in cartitem)
            {
                _context.Carts.Remove(c);
            }
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
        //Xoá toàn bộ gỏ hàng
        public IActionResult RemoveAllCart()
        {
            string username = HttpContext.Session.GetString("AccountUsername");
            int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
            Cart cart = _context.Carts.FirstOrDefault(c => c.AccountId == accountId);
            var cartitem = _context.Carts;

            foreach (Cart c in cartitem)
            {
                _context.Carts.Remove(c);
            }
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
        [HttpPost]
        [Route("/UpdateCart/{productid:int}", Name = "UpdateCart")]
        public IActionResult UpdateCart(int productid, int quantity)
        {
            Cart cart = _context.Carts.FirstOrDefault();
            var cartitem = _context.Carts.Where(p => p.Product.Id == productid);
            foreach (Cart c in cartitem)
            {
                c.Quantity = quantity;
            }
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
        public IActionResult UpdateProduct(int productId, int quantity)
        {
            if (HttpContext.Session.GetString("AccountUsername") != null)
            {
                string username = HttpContext.Session.GetString("AccountUsername");
                int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
                Cart cart = _context.Carts.FirstOrDefault(c => c.AccountId == accountId && c.ProductId == productId);
                if (cart == null)
                {
                    cart = new Cart();
                    cart.AccountId = accountId;
                    cart.ProductId = productId;
                    cart.Quantity = quantity;
                    _context.Carts.Add(cart);
                }
                else
                {
                    cart.Quantity += quantity;
                }
                _context.SaveChanges();
                return RedirectToAction("Cart");
            }
            else
            {
                return RedirectToAction("Login2", "Accounts");
            }

        }
        public IActionResult Pay()
        {
            string username = HttpContext.Session.GetString("AccountUsername");
            ViewBag.Account = _context.Accounts.Where(a => a.Username == username).FirstOrDefault();
            ViewBag.CartsTotal = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                .Where(c => c.Account.Username == username)
                                                .Sum(c => c.Quantity * c.Product.Price);
            return View("Pay");
        }
        [HttpPost]
        public IActionResult Pay([Bind("ShippingAddress,ShippingPhone")] Invoice invoice)
        {
            string username = HttpContext.Session.GetString("AccountUsername");
            if (!CheckStock(username))
            {
                ViewBag.ErrorMessage = "Có sản phẩm đã hết hàng. Vui lòng kiểm tra lại";
                ViewBag.Account = _context.Accounts.Where(a => a.Username == username).FirstOrDefault();
                ViewBag.CartsTotal = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                    .Where(c => c.Account.Username == username)
                                                    .Sum(c => c.Quantity * c.Product.Price);
                return View("Pay");
            }
            //Thêm hoá đơn
            DateTime now = DateTime.Now;
            invoice.Code = now.ToString("yyMMddhhmmss");
            invoice.AccountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
            invoice.IssuedDate = now;
            invoice.Total = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                    .Where(c => c.Account.Username == username)
                                                    .Sum(c => c.Quantity * c.Product.Price);
            _context.Add(invoice);
            _context.SaveChanges();
            //Thêm chi tiết hoá đơn
            List<Cart> carts = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                    .Where(c => c.Account.Username == username).ToList();
            foreach (Cart c in carts)
            {
                InvoiceDetail invoiceDetail = new InvoiceDetail();
                invoiceDetail.InvoiceId = invoice.Id;
                invoiceDetail.ProductId = c.ProductId;
                invoiceDetail.Quantity = c.Quantity;
                invoiceDetail.UnitPrice = c.Product.Price;
                _context.Add(invoiceDetail);
            }
            _context.SaveChanges();
            //Trừ số lượng tồn kho và xoá giỏ hàng
            foreach (Cart c in carts)
            {
                c.Product.Stock -= c.Quantity;
                _context.Carts.Remove(c);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");

        }
        private bool CheckStock(string username)
        {
            List<Cart> carts = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                    .Where(c => c.Account.Username == username).ToList();
            foreach (Cart c in carts)
            {
                if (c.Product.Stock < c.Quantity)
                {
                    return false;
                }
            }
            return true;
        }
       
        public IActionResult thongtin()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
          
    }
}
