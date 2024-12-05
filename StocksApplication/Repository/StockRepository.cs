using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StocksApplication.Data;
using StocksApplication.Dtos.Stock;
using StocksApplication.Helpers;
using StocksApplication.Interfaces;
using StocksApplication.Models;

namespace StocksApplication.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext appDbContext;
        public StockRepository(AppDbContext appContext)
        {
            this.appDbContext = appContext;
        }
        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {

            var stocks = appDbContext.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if(!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if(!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if(query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageNumber).ToListAsync();

        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await appDbContext.Stocks.Include(c =>c.Comments).FirstOrDefaultAsync(i => i.Id == id);    
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await appDbContext.Stocks.AddAsync(stockModel);
            await appDbContext.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(UpdateStockRequestDto updateStockRequestDto,int id)
        {
            var existingStockmodel = await appDbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingStockmodel == null)
            {
                return null;
            }

            existingStockmodel.Symbol = updateStockRequestDto.Symbol;
            existingStockmodel.CompanyName = updateStockRequestDto.CompanyName;
            existingStockmodel.Purchase = updateStockRequestDto.Purchase;
            existingStockmodel.LastDiv = updateStockRequestDto.LastDiv;
            existingStockmodel.Industry = updateStockRequestDto.Industry;
            existingStockmodel.MarketCap = updateStockRequestDto.MarketCap;

            await appDbContext.SaveChangesAsync();

            return existingStockmodel;
        }
        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await appDbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (stockModel == null)
            {
                return null;
            }

            appDbContext.Remove(stockModel);

            await appDbContext.SaveChangesAsync();

            return stockModel;

        }

        public Task<bool> StockExists(int id)
        {
            return appDbContext.Stocks.AnyAsync(x => x.Id == id);   
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await appDbContext.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }
    }
}
