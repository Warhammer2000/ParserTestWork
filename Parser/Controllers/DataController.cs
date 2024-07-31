using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Parser.Db;
using Parser.Models;
using Parser.Services;

namespace Parser.Controllers
{

    [Route("[controller]")]
    public class DataController : Controller
    {
        private readonly AppDbContext _context;
        private readonly DataParser _dataParser;

        public DataController(AppDbContext context, DataParser dataParser)
        {
            _context = context;
            _dataParser = dataParser;
        }

        [HttpGet]
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            _context.ChangeTracker.Clear();

            await _dataParser.ParseAndSaveData("", 5);

            var results = await _context.Purchases.ToListAsync();
            foreach (var purchase in results)
            {
                Console.WriteLine($"Проверка данных: {purchase.PurchaseNumber}, Дата окончания: {purchase.EndDate}");
            }
            return View(results);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchPhrase, int pageCount = 5)
        {
            _context.ChangeTracker.Clear();

            if (!string.IsNullOrEmpty(searchPhrase))
            {
                var filteredResults = await _context.Purchases
                    .Where(p => EF.Functions.ILike(p.Title, $"%{searchPhrase}%"))
                    .ToListAsync();
                return View(filteredResults);
            }

            await _dataParser.ParseAndSaveData(searchPhrase, pageCount);

            var results = await _context.Purchases.ToListAsync();
            return View(results);
        }
     
    }
}
