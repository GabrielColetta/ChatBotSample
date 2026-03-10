using SampleAI.Shared.Models;

namespace SampleAI.Application.Services;

public interface IChatService
{
    IAsyncEnumerable<ChatHistoryResponse> GenerateResponseAsync(Guid? chatId, string content, CancellationToken cancellationToken);
}
