using api.Dtos.Stock;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using StocksApplication.Interfaces;
using StocksApplication.Mappers;
using StocksApplication.Models;
using System.Reflection.Metadata.Ecma335;

namespace StocksApplication.Service
{
    public class FMPService : IFMPService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public FMPService(HttpClient httpClient, IConfiguration config)
        {
            this._httpClient = httpClient;
            this._config = config;
        }

    public async Task<Stock> FindStockBySymbolAsync(string symbol)
        {
           try
            {
                var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={_config["FMPKey"]}");
                if(result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var tasks = JsonConvert.DeserializeObject<FMPStock[]>(content);
                    var stock = tasks[0];
                    if(stock != null)
                    {
                        return stock.ToStockFromFMP();
                    }
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            return null;
        }
         
    }
}
