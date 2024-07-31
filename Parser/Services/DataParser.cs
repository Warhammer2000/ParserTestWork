using HtmlAgilityPack;
using Parser.Db;
using Parser.Models;

namespace Parser.Services
{
    public class DataParser
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public DataParser(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public async Task ParseAndSaveData()
        {
            var baseUrl = "https://www.roseltorg.ru/procedures/search?sale=1&status%5B%5D=5&status%5B%5D=0&status%5B%5D=1&currency=all";

            for (int i = 1; i <= 5; i++)
            {
                var url = $"{baseUrl}&page={i}";
                var html = await _httpClient.GetStringAsync(url);
                var purchases = ParseHtml(html);

                _context.Purchases.AddRange(purchases);
                await _context.SaveChangesAsync();
            }
        }

        private List<Purchase> ParseHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var purchases = new List<Purchase>();

            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='search-results__item']");
            if (nodes == null) return purchases;

            foreach (var node in nodes)
            {
                var purchase = new Purchase
                {
                    PurchaseNumber = GetNodeText(node, ".//div[@class='search-results__lot']/a"),
                    Title = GetNodeText(node, ".//div[@class='search-results__subject']/a"),
                    Organizer = GetNodeText(node, ".//div[@class='search-results__customer']/p"),
                    Price = GetNodeText(node, ".//div[@class='search-results__sum']/p"),
                    EndDate = GetNodeText(node, ".//div[@class='search-results__infoblock search-results__finish-time']/p"),
                    Location = GetNodeText(node, ".//div[@class='search-results__region']/p")
                };

                purchases.Add(purchase);
            }

            return purchases;
        }

        private string GetNodeText(HtmlNode node, string xpath)
        {
            var selectedNode = node.SelectSingleNode(xpath);
            return selectedNode?.InnerText.Trim();
        }

    }
}
