using ChessApp.API.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChessApp.API.Queries.Openings;
using ChessApp.API.Handlers.Openings;

namespace ChessApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/openings")]
public sealed class OpeningsController : ControllerBase
{
    [HttpGet("{openingId:int}")]
    public async Task<IActionResult> Get(int openingId,[FromServices] GetOpeningDetailsQuery query)
    {
        var userId = UserContext.GetUserId(User);
        var result = await query.Execute(userId, openingId);
        return Ok(result);
    }

    [HttpDelete("{openingId:int}")]
    public async Task<IActionResult> Delete(int openingId,[FromServices] DeleteOpeningHandler handler)
    {
        var userId = UserContext.GetUserId(User);
        await handler.Execute(userId, openingId);
        return Ok();
    }
}
