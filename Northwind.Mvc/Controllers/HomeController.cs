using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Northwind.Mvc.Models;
using Packt.Shared;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NorthwindContext _db;
    
    public HomeController(ILogger<HomeController> logger, NorthwindContext injectedContext)
    {
        _logger = logger;
        _db = injectedContext;
    }

    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index()
    {
        _logger.LogError("This is a serious error");
        _logger.LogWarning("This is a warning");
        _logger.LogInformation("I am in the Index method of the HomeController");

        HomeIndexViewModel model = new
        (
            VisitorCount: (new Random().Next(1, 1000)),
            Categories: await _db.Categories.ToListAsync(),
            Products: await _db.Products.ToListAsync()
        );
        
        return View(model);
    }

    public async Task<IActionResult> ProductDetail(int? id)
    {
        if (!id.HasValue)
            return BadRequest("You must pass a product id");
        
        Product? model = await _db.Products
            .SingleOrDefaultAsync(p => p.ProductId == id);
        
        if (model is null)
            return NotFound($"Product with id {id} not found");
        
        return View(model);
    }

    [Route("private")]
    [Authorize(Roles = "Administrators")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ModelBinding()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ModelBinding(Thing thing)
    {
        HomeModelBindingViewModel model = new
        (
            thing,
            !ModelState.IsValid,
            ModelState.Values
                .SelectMany(s => s.Errors)
                .Select(e => e.ErrorMessage)
        );
        return View(model);
    }

    public IActionResult ProductsThatCostMoreThan(decimal? price)
    {
        if (!price.HasValue)
            return BadRequest("You must pass a price");

        IEnumerable<Product> model = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.UnitPrice > price);
        
        if (!model.Any())
            return NotFound("No products found");

        ViewData["MaxPrice"] = price.Value.ToString();

        return View(model);
    }
}