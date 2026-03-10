using MediatR;
using SampleAI.Domain.Entities;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;

namespace SampleAI.Application.Contracts.Queries;

public record GetConversationQuery(PaginateFilter Filter, Guid ChatId) : IRequest<PaginatedResponse<Conversation>>;
