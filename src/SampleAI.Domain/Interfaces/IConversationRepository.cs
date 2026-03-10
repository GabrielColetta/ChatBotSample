using SampleAI.Domain.Entities;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;

namespace SampleAI.Domain.Interfaces;

public interface IConversationRepository : IPaginatable<Conversation>
{
    Task CreateAsync(Conversation entity, CancellationToken cancellationToken);

    Task<PaginatedResponse<Conversation>> VectorSearchAsync(
        PaginateFilter paginateFilter,
        float[] embedding,
        CancellationToken cancellationToken);
}
