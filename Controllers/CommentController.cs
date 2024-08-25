using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comments;
using api.Extentions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [EnableCors("AllowFrontend")]
    // This attribute specifies the route template for the controller.
    // The [controller] token will be replaced with the controller name, which in this case is "Comment".
    [Route("api/[controller]")]

    // This attribute indicates that the controller is an API controller.
    // It enables automatic model validation and binds parameters from the request body.
    [ApiController]

    public class CommentController : ControllerBase
    {
        // Controller code goes here.
        private readonly UserManager<AppUser> _userManager;
        private readonly ICommentsRepository _commentsRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(UserManager<AppUser> userManager, ICommentsRepository commentsRepo, IStockRepository stockRepo)
        {
            _userManager = userManager;
            _commentsRepo = commentsRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comments = await _commentsRepo.GetALLAsync();
            var commentDtos = comments.Select(comment => comment.ToCommentDto());
            return Ok(commentDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var comment = await _commentsRepo.GetByIDAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }
        

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> CreateComment([FromRoute] int stockId, CreateCommentDto commentDto)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _stockRepo.StockExists(stockId))
            {
                return BadRequest("Stock dose not exist");
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);


            var commentModel = commentDto.ToCommentFromCreateDTO(stockId);

            commentModel.AppUserId = appUser.Id;

            await _commentsRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetCommentById), new { id = commentModel.Id }, commentModel.ToCommentDto());

        }


 // Uncomment and correct the route definition
    [HttpPost("{symbol}")] // Remove the :string constraint
    public async Task<IActionResult> CreateCommentBySymbol([FromRoute] string symbol, CreateCommentDto commentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var stock = await _stockRepo.GetBySymbolAsync(symbol);
        
        // Uncomment the following code block to enable logging
        // Console.WriteLine("Logging to console");
        System.Diagnostics.Debug.WriteLine("stock:" + stock?.Symbol);
        if (stock == null)
        {
            return BadRequest("Stock does not exist");
        }

        var username = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(username);

        var commentModel = commentDto.ToCommentFromCreateDTO(stock.Id);
        commentModel.AppUserId = appUser.Id;

        await _commentsRepo.CreateAsync(commentModel);
        return CreatedAtAction(nameof(GetCommentById), new { id = commentModel.Id }, commentModel.ToCommentDto());
    }
        // [HttpPost("{symbol:string}")]
        // public async Task<IActionResult> CreateCommentBySymbol([FromRoute] string symbol, CreateCommentDto commentDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
            
        //     var stock = await _stockRepo.GetBySymbolAsync(symbol);
            
        //     // Uncomment the following code block to enable logging
        //     // Console.WriteLine("Logging to console");
        //     System.Diagnostics.Debug.WriteLine("stock:"+ stock?.Symbol);
        //     if (stock == null)
        //     {
        //         return BadRequest("Stock does not exist");
        //     }

        //     var username = User.GetUsername();
        //     var appUser = await _userManager.FindByNameAsync(username);

        //     var commentModel = commentDto.ToCommentFromCreateDTO(stock.Id);
        //     commentModel.AppUserId = appUser.Id;

        //     await _commentsRepo.CreateAsync(commentModel);
        //     return CreatedAtAction(nameof(GetCommentById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        // }


        

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var comment = await _commentsRepo.UpdateAsync(id, commentDto.ToCommentFromUpdate());
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var comment = await _commentsRepo.DeleteAsync(id);
            if (comment == null)
            {
                return NotFound("Comment not found");
            }
            return NoContent();
        }



    }
}