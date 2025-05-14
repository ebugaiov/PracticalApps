using Microsoft.AspNetCore.Mvc.RazorPages;  // PageModel
using Packt.Shared;  // NorthwindContext, Employee

namespace PacktFeatures.Pages;

public class EmployeesPageModel : PageModel
{
    private NorthwindContext _db;

    public EmployeesPageModel(NorthwindContext injectedContext)
    {
        _db = injectedContext;
    }

    public Employee[] Employees { get; set; } = null!;

    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Employees";
        Employees = _db.Employees
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToArray();
    }
}
