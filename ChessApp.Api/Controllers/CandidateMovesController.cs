using ChessApp.API.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChessApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/candidate-moves")]
public sealed class CandidateMovesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string fen, [FromQuery] int? currentOpeningId, [FromServices] GetCandidateMovesQuery query)
    {
        var userId = UserContext.GetUserId(User);
        var result = await query.Execute(userId, fen, currentOpeningId);

        return Ok(result);
    }
}
