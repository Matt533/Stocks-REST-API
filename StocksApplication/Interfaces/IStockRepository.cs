using StocksApplication.Dtos.Stock;
using StocksApplication.Helpers;
using StocksApplication.Models;

namespace StocksApplication.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync(QueryObject query);
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<Stock>  CreateAsync (Stock stockModel);
        Task<Stock?> UpdateAsync (UpdateStockRequestDto updateStockDto, int id);
        Task<Stock?> DeleteAsync (int id);
        Task<bool> StockExists(int id);
    }
}
