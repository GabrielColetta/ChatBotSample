using MediatR;
using SampleAI.Domain.Entities;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;

namespace SampleAI.Application.Contracts.Queries;

public record GetChatByVectorSearchQuery(PaginateFilter Filter, string Search) : IRequest<PaginatedResponse<Conversation>>;
