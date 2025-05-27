namespace WordCloudService.Tests;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WordCloudService.Models;
using WordCloudService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using WordCloudService.Controllers;
using Xunit;

public class WordCloudeServiceTests
{
    [Fact(DisplayName = "GenerateAsync sends the correct request and returns bytes")]
    public async Task GenerateAsync_ReturnsBytesOnSuccess()
    {
        var words = new[] { new WordDto { Text = "a", Weight = 1 }, new WordDto { Text = "b", Weight = 2 } };
        var request = new CloudRequestDto { Width = 100, Height = 200, FontScale = 2, Scale = "linear", Words = new System.Collections.Generic.List<WordDto>(words) };
        var expected = new byte[] { 1, 2, 3 };
        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(expected) };
        var handler = new FakeHandler(response);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://test/") };
        var svc = new WordCloudeService(client);

        var result = await svc.GenerateAsync(request);

        Assert.Equal(expected, result);
        Assert.Equal(HttpMethod.Post, handler.Request.Method);
        Assert.Equal("/wordcloud", handler.Request.RequestUri.PathAndQuery);
        var json = await handler.Request.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        Assert.Equal("a:1,b:2", root.GetProperty("text").GetString());
        Assert.Equal("png", root.GetProperty("format").GetString());
        Assert.Equal(100, root.GetProperty("width").GetInt32());
        Assert.Equal(200, root.GetProperty("height").GetInt32());
        Assert.Equal(2, root.GetProperty("fontScale").GetInt32());
        Assert.Equal("linear", root.GetProperty("scale").GetString());
        Assert.True(root.GetProperty("useWordList").GetBoolean());
    }

    [Fact(DisplayName = "GenerateAsync throws an exception on an unsuccessful status")]
    public async Task GenerateAsync_ThrowsOnErrorStatus()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var handler = new FakeHandler(response);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://test/") };
        var svc = new WordCloudeService(client);

        await Assert.ThrowsAsync<HttpRequestException>(() => svc.GenerateAsync(new CloudRequestDto()));
    }

    private class FakeHandler : HttpMessageHandler
    {
        public HttpRequestMessage Request;
        private readonly HttpResponseMessage _resp;
        public FakeHandler(HttpResponseMessage resp) => _resp = resp;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(_resp);
        }
    }
}

public class CloudControllerTests
{
    [Fact(DisplayName = "Post returns a FileContentResult with content type image/png")]
    public async Task Post_Success_ReturnsFileContentResult()
    {
        var data = new byte[] { 1, 2 };
        var svc = new StubService(data);
        var ctrl = new CloudController(svc, NullLogger<CloudController>.Instance);

        var result = await ctrl.Post(new CloudRequestDto()) as FileContentResult;

        Assert.Equal("image/png", result.ContentType);
        Assert.Equal(data, result.FileContents);
    }

    private class StubService : IWordCloudeService
    {
        private readonly byte[] _data;
        public StubService(byte[] data) => _data = data;
        public Task<byte[]> GenerateAsync(CloudRequestDto request) => Task.FromResult(_data);
    }
}

public class ModelsTests
{
    [Fact(DisplayName = "CloudRequestDto has the correct default values")]
    public void CloudRequestDto_DefaultValues()
    {
        var dto = new CloudRequestDto();
        Assert.Equal("png", dto.Format);
        Assert.Equal("linear", dto.Scale);
        Assert.NotNull(dto.Words);
        Assert.Empty(dto.Words);
    }

    [Fact(DisplayName = "WordDto has the correct default properties")]
    public void WordDto_DefaultProperties()
    {
        var w = new WordDto();
        Assert.Equal(string.Empty, w.Text);
        Assert.Equal(0, w.Weight);
    }
}
