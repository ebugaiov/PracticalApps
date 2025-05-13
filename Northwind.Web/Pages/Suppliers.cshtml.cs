using Microsoft.AspNetCore.Mvc.RazorPages;  // PageModel
using Microsoft.AspNetCore.Mvc;  // [BindProperty], IActionResult
using Packt.Shared;  // NorthwindContext

namespace Northwind.Web.Pages;

public class SuppliersModel : PageModel
{
    private NorthwindContext _db;
    public IEnumerable<Supplier>? Suppliers { get; set; }

    public SuppliersModel(NorthwindContext injectedContext)
    {
        _db = injectedContext;
    }
    
    public void OnGet()
    {
        ViewData["Title"] =  "Northwind B2B - Suppliers";
        Suppliers = _db.Suppliers
            .OrderBy(c => c.Country)
            .ThenBy(c => c.CompanyName);
    }
    
    [BindProperty]
    public Supplier? Supplier { get; set; }

    public IActionResult OnPost()
    {
        if ((Supplier is not null) && ModelState.IsValid)
        {
            _db.Suppliers.Add(Supplier);
            _db.SaveChanges();
            return RedirectToPage("/Suppliers");
        }
        else
        {
            return Page();
        }
    }
}