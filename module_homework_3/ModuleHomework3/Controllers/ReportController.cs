using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuleHomework3.Data;

namespace ModuleHomework3.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var report1 = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .Select(p => new { p.Name, CategoryName = p.Category!.Name, p.Price })
                .ToListAsync();

            var report2 = await _context.Products
                .GroupBy(p => p.Category!.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderBy(r => r.Category)
                .ToListAsync();

            var report3 = await _context.Products
                .GroupBy(p => p.Category!.Name)
                .Select(g => new { Category = g.Key, AvgPrice = g.Average(p => p.Price) })
                .OrderByDescending(r => r.AvgPrice)
                .ToListAsync();

            ViewBag.Report1 = report1;
            ViewBag.Report2 = report2;
            ViewBag.Report3 = report3;

            return View();
        }
    }
}
