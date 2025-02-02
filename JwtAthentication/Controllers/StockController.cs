using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecondApp.Data;
using SecondApp.Dtos.Stock;
using SecondApp.Interfaces;
using SecondApp.Mappers;
using SecondApp.Models;

namespace SecondApp.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockRepository _stockRepo;
        public StockController(ApplicationDbContext context, IStockRepository StockRepo) { 
            _context = context;
            _stockRepo = StockRepo; 
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var stocks = await _stockRepo.GetAllAsync();
            var stocksDto = stocks. 
                Select(s =>
                {
                    return s.ToStockDto();
                });

            if (!stocks.Any())
            {
                return NotFound("there is no stock yet!");
            }
            return Ok(stocksDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {

            Stock? stock = await _stockRepo.GetByIdAsync(id);



            if (stock == null)
            {
                return NotFound();
            }
            else
            {
            StockDto stockDto = stock.ToStockDto();
            return Ok(stock);
            }
        }

        [HttpPost]
        public async Task< IActionResult > CreateStock([FromBody] CreateStockDto stockData)
        {
            Stock stock = new Stock()
            {
                Symbol = stockData.Symbol,
                divdend = stockData.divdend,
                CompanyName = stockData.CompanyName,
                purchase = stockData.purchase,
            };
            //var stockEntity = await _context.Stocks.AddAsync(stock);
            //await _context.SaveChangesAsync();
            //return Ok(stock.ToStockDto());
            var stockEntity = await _stockRepo.CreateStockAsync(stock);
            return CreatedAtAction(nameof(GetById), new { id = stock.Id }, stock.ToStockDto());
        }



        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockDto stockModel)
        {
            Stock? stock = await _stockRepo.UpdateStockAsync(id, stockModel);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }


    }
}
