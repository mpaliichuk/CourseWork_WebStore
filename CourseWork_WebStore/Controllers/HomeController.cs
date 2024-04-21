using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CourseWork_WebStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseWork_WebStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StoreDbContext _context;

    public HomeController(ILogger<HomeController> logger, StoreDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(string categoryName)
    {
        if (User.Identity.IsAuthenticated)
        {
            ViewBag.UserName = User.Identity.Name;
        }
        else
        {
            ViewBag.UserName = "Guest";
        }
        var categories = await _context.Categories.ToListAsync();

        ViewBag.Categories = categories;

        IQueryable<Product> productsQuery = _context.Products;

        if (!string.IsNullOrEmpty(categoryName))
        {
            productsQuery = productsQuery.Where(p => p.Category.Name == categoryName);
        }

        var products = await productsQuery.ToListAsync();

        return View(products);
    }



    //private readonly StoreDbContext _context;

    //public ProductsController(StoreDbContext context)
    //{
    //    _context = context;
    //}

    //// GET: Products
    //public async Task<IActionResult> Index()
    //{
    //    var products = await _context.Products.ToListAsync();
    //    return View(products);
    //}


    [Authorize(Roles ="Admin")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

