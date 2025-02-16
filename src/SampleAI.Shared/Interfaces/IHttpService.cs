
namespace SampleAI.Shared.Interfaces;

public interface IHttpService
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken) where TResponse : class;
}
