﻿using Microsoft.EntityFrameworkCore;
using Parser.Db;

namespace Parser.Services
{
    public class DataService
    {
        private readonly AppDbContext _context;

        public DataService(AppDbContext appDb)
        {
            _context = appDb;
        }
        public async Task DeleteAllDataAsync()
        {
            _context.Purchases.RemoveRange(_context.Purchases);

            await _context.SaveChangesAsync();
        }
    }
}