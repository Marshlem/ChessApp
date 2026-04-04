using System.Security.Claims;
using ChessApp.Application.Training.GetNextTrainingPosition;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChessApp.Application.Training.SubmitTrainingAnswer;
using ChessApp.Application.Training.GetTrainingSummary;

namespace ChessApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/training")]
public sealed class TrainingController : ControllerBase
{
    private readonly IMediator _mediator;

    public TrainingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("next-position")]
    public async Task<IActionResult> GetNextPosition(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _mediator.Send(new GetNextTrainingPositionQuery(userId), cancellationToken);

        if (result is null)
        {
            return NoContent();
        }

        return Ok(result);
    }

    [HttpPost("submit-answer")]
    public async Task<IActionResult> SubmitAnswer(
    [FromBody] SubmitTrainingAnswerRequest request,
    CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _mediator.Send(
            new SubmitTrainingAnswerCommand(
                userId,
                request.OpeningNodeId,
                request.SelectedOpeningNodeId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _mediator.Send(new GetTrainingSummaryQuery(userId), cancellationToken);

        return Ok(result);
    }
}