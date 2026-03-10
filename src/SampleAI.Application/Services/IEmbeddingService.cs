namespace SampleAI.Application.Services;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingFromModelAsync(string text, CancellationToken cancellationToken);
}
