using ChessApp.API.Infrastructure;
using ChessApp.API.DTOs.OpeningNodes;
using ChessApp.API.Handlers.OpeningNodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChessApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/openings/{openingId:int}/nodes")]
public sealed class OpeningNodesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddMove(int openingId, [FromBody] AddMoveRequest request, [FromServices] AddMoveHandler handler)
    {
        var userId = UserContext.GetUserId(User);
        var result = await handler.Execute(userId, openingId, request);
        return Ok(result);
    }

    [HttpDelete("{nodeId:int}")]
    public async Task<IActionResult> DeleteSubtree(int openingId, int nodeId, [FromServices] DeleteOpeningNodeSubtreeHandler handler)
    {
        var userId = UserContext.GetUserId(User);

        await handler.Execute(userId, openingId, nodeId);

        return Ok();
    }

    [HttpPatch("{nodeId:int}/type")]
    public async Task<IActionResult> UpdateNodeType(int openingId, int nodeId, [FromBody] UpdateNodeTypeRequest request, [FromServices] UpdateNodeTypeHandler handler)
    {
        var userId = UserContext.GetUserId(User);

        var result = await handler.Execute(userId, openingId, nodeId, request);

        return Ok(result);
    }

    [HttpPatch("{nodeId:int}/comment")]
    public async Task<IActionResult> UpdateComment(
        int openingId,
        int nodeId,
        [FromBody] UpdateOpeningNodeCommentRequest request,
        [FromServices] UpdateOpeningNodeCommentHandler handler,
        CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        await handler.Execute(userId, openingId, nodeId, request, cancellationToken);

        return Ok();
    }

    [HttpPatch("{nodeId:int}/moveEvaluation")]
    public async Task<IActionResult> UpdateMoveEvaluation(
        int openingId,
        int nodeId,
        [FromBody] UpdateOpeningNodeMoveEvaluationRequest request,
        [FromServices] UpdateOpeningNodeMoveEvaluationHandler handler,
        CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        await handler.Execute(userId, openingId, nodeId, request, cancellationToken);

        return Ok();
    }
}
