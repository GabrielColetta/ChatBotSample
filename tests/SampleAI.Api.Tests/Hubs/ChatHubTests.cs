using AutoFixture.Xunit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SampleAI.Api.Models.Responses;

namespace SampleAI.Api.Tests.Hubs;

[Collection("Tests")]
public class ChatHubTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ChatHubTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [AutoData]
    public async Task Should_ReturnNewContent_When_GetHistoryAsync(string userPrompt, Guid conversationId)
    {
        await using var scoped = _factory.Services.CreateAsyncScope();
        var chatHistoryMock = scoped.ServiceProvider.GetRequiredService<IChatClient>();
        chatHistoryMock.GetStreamingResponseAsync(Arg.Any<IList<ChatMessage>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(GetResponse().ToAsyncEnumerable());

        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost/chat", options =>
            {
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents;
            })
            .Build();

        var response = string.Empty;
        connection.On<HistoryResponse>("ReceiveToken", r =>
        {
            response += r.Content;
        });

        await connection.StartAsync(CancellationToken.None);

        await connection.InvokeAsync("SendMessageAsync", userPrompt, conversationId, CancellationToken.None);

        await Task.Delay(2000, CancellationToken.None);

        response.Should().Be("Hello there");
    }

    private static IEnumerable<ChatResponseUpdate> GetResponse()
    {
        return
        [
            new ChatResponseUpdate(ChatRole.Assistant, "Hello there")
        ];
    }
}
