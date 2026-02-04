using ChessApp.API.Models;
using ChessApp.API.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChessApp.API.DTOs.Repertoire;
using ChessApp.API.Handlers.Repertoire;

namespace ChessApp.API.Controllers;

[ApiController]
[Route("api/repertoire")]
public sealed class RepertoireController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RepertoireItemDto>>> GetTree([FromServices] GetRepertoireTreeQuery query)
    {
        var userId = UserContext.GetUserId(User);
        return Ok(await query.Execute(userId));
    }

    [HttpPost("opening")]
    public async Task<IActionResult> CreateOpening([FromBody] CreateOpeningRequest request,[FromServices] CreateOpeningHandler handler)
    {
        var userId = UserContext.GetUserId(User);
        var openingId = await handler.Execute(userId, request);
        return Ok(openingId);
    }
}
