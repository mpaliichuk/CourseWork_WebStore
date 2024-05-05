using CourseWork_WebStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork_WebStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly StoreDbContext _context;

        public OrderController(StoreDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("Role");

            if (userRole == "Admin")
            {
                var orders = await _context.Orders.ToListAsync();
                return View(orders);
            }
            else
            {
                var orders = await _context.Orders
                                            .Where(o => o.UserId == userId)
                                            .ToListAsync();

                return View(orders);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CreateOrder(int userId, decimal totalAmount, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            totalAmount = product.Price * quantity;

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = totalAmount
            };

            var orderItem = new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price
            };

            product.Stock -= quantity;

            order.OrderItems = new List<OrderItem> { orderItem };

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.OrderItems)
                                      .ThenInclude(oi => oi.Product)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == "Pending")
            {
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Product.Stock += orderItem.Quantity;
                }

                order.Status = "Cancelled";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "This order cannot be cancelled.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.OrderItems)
                                      .ThenInclude(oi => oi.Product)
                                      .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != "Pending")
            {
                return BadRequest("Cannot delete order. Only pending orders can be deleted.");
            }

            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Product.Stock += orderItem.Quantity;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult ShowOrderItems(int orderId)
        {
            var orderItems = _context.OrderItems
                                    .Include(oi => oi.Product)
                                    .Where(oi => oi.OrderId == orderId)
                                    .ToList();

            return View(orderItems);
        }


    }
}