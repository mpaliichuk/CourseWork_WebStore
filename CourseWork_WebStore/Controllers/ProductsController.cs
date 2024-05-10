using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseWork_WebStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork_WebStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StoreDbContext _context;

        public ProductsController(IHttpContextAccessor httpContextAccessor,StoreDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        // GET: Products
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
        // GET: Products/Edit/5
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(product);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
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

        // GET: Products/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(ReviewViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            var userRole = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userRole) || (userRole != "Admin" && userRole != "User"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var review = new Review
            {
                UserId = userId.Value,
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("WriteReview", new { productId = model.ProductId });
        }

        public IActionResult WriteReview(int productId)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var userId = session.GetInt32("UserId");

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            var reviews = _context.Reviews.Where(r => r.ProductId == productId).ToList();

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            var allUsers = _context.Users.ToList();

            ViewData["Product"] = product;
            ViewData["UserId"] = userId;
            ViewData["Reviews"] = reviews;
            ViewData["User"] = user;
            ViewData["AllUsers"] = allUsers;

            return View();
        }




    }
}