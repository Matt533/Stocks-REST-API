using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StocksApplication.Data;
using StocksApplication.Dtos.Stock;
using StocksApplication.Helpers;
using StocksApplication.Interfaces;
using StocksApplication.Mappers;
using StocksApplication.Models;

namespace StocksApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private  readonly IStockRepository _stockRepository;
        public StockController(IStockRepository stockRepository)
        {
            this._stockRepository = stockRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllStocks([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stocks = await _stockRepository.GetAllAsync(query);

             var stockDto =  stocks.Select(s => s.ToStockDto()).ToList() ;

            return Ok(stockDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockById( [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stock = await _stockRepository.GetByIdAsync(id);

            if(stock is null)
            {
                return NotFound("Id doesn't exist!");
            }

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto createStockDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockModel =  createStockDto.ToStockFromCreateDto();

            await _stockRepository.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetStockById), new { id = stockModel.Id },
                stockModel.ToStockDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id,[FromBody] UpdateStockRequestDto updateStockDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockModel = await _stockRepository.UpdateAsync(updateStockDto, id);
         
            if(stockModel is null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete]
        [Route ("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockModel = await _stockRepository.DeleteAsync(id);

            if(stockModel is null)
            {
                return NotFound();
            }

            return NoContent();

        }
    }
}
