using MediatR;
using SampleAI.Domain.Entities;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;

namespace SampleAI.Application.Contracts.Queries;

public record GetChatPaginatedQuery(PaginateFilter PaginateFilter) : IRequest<PaginatedResponse<Chat>>;
