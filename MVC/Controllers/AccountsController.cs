using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using Microsoft.AspNetCore.Http;


namespace MVC.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ShopContext _context;

        public AccountsController(ShopContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Accounts.ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ByAddress()
        {
            var dc = _context.Accounts.Where(ad => ad.Address.Contains("Tp.Hồ Chí Minh")).ToList();
            return View(dc);
        }
        public IActionResult ByEmail()
        {
            var email = _context.Accounts.Where(acc => acc.Email.Contains("@gmail")).ToList();
            return View(email);
        }
        public IActionResult Search(string username, string email, string phone, string address, string fullname)
        {
            List<Account> accounts = _context.Accounts.ToList();
            if (username != null)
            {
                accounts = accounts.Where(acc => acc.Username == username).ToList();
            }
            if (email != null)
            {
                accounts = accounts.Where(acc => acc.Email == email).ToList();
            }
            if (phone != null)
            {
                accounts = accounts.Where(acc => acc.Phone == phone).ToList();
            }
            if (address != null)
            {
                accounts = accounts.Where(acc => acc.Address.Contains(address)).ToList();
            }
            if (fullname != null)
            {
                accounts = accounts.Where(acc => acc.FullName.Contains(fullname)).ToList();
            }
            return View(accounts);
        }
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            bool result = _context.Accounts.Where(a => a.Username == Username && a.Password == Password).Count() > 0;
            if (result)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Dang nhap that bai";
                return View();
            }
            return View();
        }
        public IActionResult login2()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login2(string Username, string Password)
        {
            Account account = _context.Accounts.Where(a => a.Username == Username
                && a.Password == Password).FirstOrDefault();
            if (account != null)
            {
                /*CookieOptions cookieOptions = new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(7)
                };
                HttpContext.Response.Cookies.Append("AccountID", account.Id.ToString(),cookieOptions);
                HttpContext.Response.Cookies.Append("AccountUsername", account.Username.ToString(),cookieOptions);*/
                var a = _context.Accounts.Where(a => a.Username == Username
                 && a.Password == Password).Select(a=>a.IsAdmin).FirstOrDefault();
                if(a == true)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    HttpContext.Session.SetInt32("AccountID", account.Id);
                    HttpContext.Session.SetString("AccountUsername", account.Username);
                    return RedirectToAction("Index", "Home");
                }  
            }
            else
            {
                ViewBag.ErrorMessage = "Dang nhap that bai";
                return View();
            }
            return View();
        }
        //Xử lí logout
        public IActionResult Logout()
        {
            //Cookie
            /*HttpContext.Response.Cookies.Append("AccountID","",
                new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            HttpContext.Response.Cookies.Append("AccountUsername", "",
                new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            */
            //Huỷ Session
            //Huỷ 1 thành phần trong session
            HttpContext.Session.Remove("AccountID");
            //Huỷ toàn bộ session
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");

        }
        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
