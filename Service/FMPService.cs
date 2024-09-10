using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stocks;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Newtonsoft.Json;

namespace api.Service
{
    public class FMPService : IFMPService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        // private readonly string _baseUrl = "https://financialmodelingprep.com/api/v3/";
        public FMPService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public async Task<Stock> FindStockBySymbolAsync(string symbol)
        {
            try{
                var result = await _httpClient.GetAsync($"{_config["FMP:URL"]}{symbol}?apikey={_config["FMP:ApiKey"]}");
                if(result.IsSuccessStatusCode){
                    var content = await result.Content.ReadAsStringAsync();
                    var tasks = JsonConvert.DeserializeObject<FMPStock[]>(content);
                    var stock = tasks[0];
                    if(stock != null)
                    {
                        return stock.ToStockFromFMP();
                    }
                    return null;
                }
                return null;
            }catch(Exception ex){
                Console.WriteLine(ex);
                return null;
            }

        }
    }
}