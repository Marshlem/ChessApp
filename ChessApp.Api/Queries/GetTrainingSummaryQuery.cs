using MediatR;
using ChessApp.Application.Training.GetNextTrainingPosition;

namespace ChessApp.Application.Training.GetTrainingSummary;

public sealed record GetTrainingSummaryQuery(int UserId) : IRequest<GetTrainingSummaryResponse>;