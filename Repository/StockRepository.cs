using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stocks;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            // we passing a model
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModle = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stockModle == null) return null;

            _context.Stocks.Remove(stockModle);
            await _context.SaveChangesAsync();
            return stockModle;
        }

        public async Task<List<Stock>> GetALLAsync(QueryObject query)
        {


            var stocks = _context.Stocks.Include(x => x.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {

                    stocks = query.IsDecsending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            // pagination 
            var skipNumber = (query.PageNumber - 1) * query.PageSize;


            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol )
        {
            return await _context.Stocks.FirstOrDefaultAsync(x => x.Symbol == symbol);

        }

        public async Task<bool> StockExists(int id)
        {
            return await _context.Stocks.AnyAsync(s => s.Id == id);

        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockDtos stockDto)
        {
            var existStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (existStock == null) return null;
            existStock.Symbol = stockDto.Symbol;
            existStock.CompanyName = stockDto.CompanyName;
            existStock.Purchase = stockDto.Purchase;
            existStock.Dividend = stockDto.Dividend;
            existStock.LastDiv = stockDto.LastDiv;
            existStock.Industry = stockDto.Industry;
            existStock.MarketCap = stockDto.MarketCap;
            await _context.SaveChangesAsync();
            return existStock;

        }
    }
}