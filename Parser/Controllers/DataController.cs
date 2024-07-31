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
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            _context.ChangeTracker.Clear();

            if (!_context.Purchases.Any())
            {
                await _dataParser.ParseAndSaveData("", 5);
            }
            await _dataParser.ParseAndSaveData("", 5);

            var totalItems = await _context.Purchases.CountAsync();
            var purchases = await _context.Purchases
                                         .OrderBy(p => p.PurchaseNumber) 
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();

            var viewModel = new PurchaseViewModel
            {
                Purchases = purchases,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                SearchPhrase = "",
                PageCount = 5
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchPhrase, int pageCount = 5, int pageNumber = 1)
        {
            _context.ChangeTracker.Clear();

            IQueryable<Purchase> query = _context.Purchases;

            if (!string.IsNullOrEmpty(searchPhrase))
            {
                query = query.Where(p => EF.Functions.ILike(p.Title, $"%{searchPhrase}%"));
            }

            int pageSize = 7; 
            var totalItems = await query.CountAsync();
            var filteredResults = await query
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var viewModel = new PurchaseViewModel
            {
                Purchases = filteredResults,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                SearchPhrase = searchPhrase,
                PageCount = pageCount
            };

            return View(viewModel);
        }

        [HttpGet("api/purchases/all")]
        public async Task<IActionResult> GetAllPurchases()
        {
            var purchases = await _context.Purchases.ToListAsync();
            return Ok(purchases);
        }

    }
}
