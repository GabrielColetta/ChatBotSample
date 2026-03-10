using Microsoft.Extensions.AI;

namespace SampleAI.Application.Services.Impl;

public class EmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public EmbeddingService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<float[]> GetEmbeddingFromModelAsync(string text, CancellationToken cancellationToken)
    {
        var embedding = await _embeddingGenerator.GenerateVectorAsync(text, cancellationToken: cancellationToken);

        return embedding.ToArray();
    }
}
