using MediatR;

namespace ChessApp.Application.Training.GetNextTrainingPosition;

public record GetNextTrainingPositionQuery(int UserId) : IRequest<GetNextTrainingPositionResponse?>;