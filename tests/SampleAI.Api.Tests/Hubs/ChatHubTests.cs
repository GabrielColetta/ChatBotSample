using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SampleAI.Api.Hubs;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;

namespace SampleAI.Api.Tests.Hubs;

public class ChatHubTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ChatHubTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [AutoData]
    public async Task Should_ReturnNewContent_When_GetHistoryAsync(string content, string conversationId)
    {
        // Given
        await using var scoped = _factory.Services.CreateAsyncScope();
        var chatHistoryMock = scoped.ServiceProvider.GetRequiredService<IChatClient>();
        chatHistoryMock.CompleteStreamingAsync(Arg.Any<IList<ChatMessage>>())
            .Returns(GetResponse().ToAsyncEnumerable());

        var connection = new HubConnectionBuilder()
            .WithUrl("wss://localhost/chat", options =>
            {
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .Build();

        await connection.StartAsync();

        // When
        var response = await connection.StreamAsync<ChatHistoryModel>("SendMessageAsync", content, conversationId).ToListAsync();

        // Then
        response.First().Content.Should().Be("Hello there");
    }

    private IEnumerable<StreamingChatCompletionUpdate> GetResponse()
    {
        return
        [
            new() {
                Text = "Hello there"
            }
        ];
    }
}
