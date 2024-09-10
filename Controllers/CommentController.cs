using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comments;
using api.Extentions;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
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
        private readonly IFMPService _fmpService;
        public CommentController(UserManager<AppUser> userManager, ICommentsRepository commentsRepo, IStockRepository stockRepo, IFMPService fmpService)
        {
            _userManager = userManager;
            _commentsRepo = commentsRepo;
            _stockRepo = stockRepo;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllComments([FromQuery] CommentQueryObject queryObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comments = await _commentsRepo.GetAllAsync(queryObject);
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


        [HttpPost("{symbol:alpha}")]
        public async Task<IActionResult> CreateComment([FromRoute] string symbol, CreateCommentDto commentDto)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var stock = await _stockRepo.GetBySymbolAsync(symbol);
            if (stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);

                // Ensure required properties are set
                if (stock == null)
                {
                    return BadRequest("Stock does not exist");
                }
                else
                {
                    await _stockRepo.CreateAsync(stock);
                }
            }


            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);


            var commentModel = commentDto.ToCommentFromCreateDTO(stock.Id);

            commentModel.AppUserId = appUser.Id;

            await _commentsRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetCommentById), new { id = commentModel.Id }, commentModel.ToCommentDto());

        }







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