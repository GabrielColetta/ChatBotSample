using MediatR;
using SampleAI.Application.Contracts.Responses;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;

namespace SampleAI.Application.Contracts.Queries;

public record GetConversationByIdQuery(PaginateFilter Filter, Guid ChatId) : IRequest<PaginatedResponse<GetConversationByIdResponse>>;
