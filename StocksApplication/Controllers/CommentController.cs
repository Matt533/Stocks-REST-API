using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StocksApplication.Data;
using StocksApplication.Dtos.Comment;
using StocksApplication.Extensions;
using StocksApplication.Interfaces;
using StocksApplication.Mappers;
using StocksApplication.Models;

namespace StocksApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFMPService _fmpService;
        public CommentController(ICommentRepository commentRepository,
            IStockRepository stockRepository,
           UserManager<AppUser> userManager, 
           IFMPService fMPService)
        {
            this._commentRepository = commentRepository;
            this._stockRepository = stockRepository;
            this._userManager = userManager;
            this._fmpService = fMPService;  
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var allComments = await _commentRepository.GetAllAsync();

            var commentDto = allComments.Select(c => c.ToCommentDto());
            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var commmentModel = await _commentRepository.GetByIdAsync(id);

            if(commmentModel == null)
            {
                return NotFound();
            }

            return Ok(commmentModel.ToCommentDto());
        }


        [HttpPost("{symbol:alpha}")]
        public async Task<IActionResult> Create([FromRoute] string symbol, CreateCommentDto commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           

            var stock = await _stockRepository.GetBySymbolAsync(symbol);

            if(stock == null)
            {
               stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if(stock == null)
                {
                    return BadRequest("Stock does not exist");
                }
                else
                {
                    await _stockRepository.CreateAsync(stock);
                }
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var commentModel = commentDto.ToCommentFromCreate(stock.Id);
            commentModel.AppUserId = appUser.Id;  
         
            await _commentRepository.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, UpdateCommentDto updateCommentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var commentModel = await _commentRepository.UpdateAsync(id, updateCommentDto.ToCommentFromUpdate());

            if (commentModel is null )
            {
                return NotFound("Comment not found");
            }

            return Ok(commentModel.ToCommentDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentRepository.DeleteAsync(id);

            if(comment is null )
            {
                return NotFound("Comment doesn't exist!");
            }

            return Ok(comment);
        }

    }
}
