using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MVCPhones.Data;
using Savonia.xUnit.Helpers;
using Xunit;
using Xunit.Abstractions;
using Savonia.xUnit.Helpers.Infrastructure;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly HttpClient _client;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture) : base(new string[] {"Phones.db"}, testOutputHelper)
    {
        _client = fixture.CreateClient();
        WriteLine(fixture.HostUrl);
    }

    [Fact]
    public async void Checkpoint02_AddLink()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/phones");
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var links = content.QuerySelectorAll("a");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotEmpty(links);
        Assert.Contains(links, a => a.GetAttribute("href")?.ToLower() == "/phones/add");
        var addLink = links.First(a => a.GetAttribute("href")?.ToLower() == "/phones/add");
        Assert.Contains("btn", addLink.GetAttribute("class"));    
    }

    [Theory]
    [JsonFileData("testdata.json", typeof(string), typeof(IEnumerable<Phone>))]
    public async void Checkpoint02_Content(string data, IEnumerable<Phone> expected)
    {
        // Arrange
        int index = 0;

        // Act
        var response = await _client.GetAsync(data);
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var ul = content.QuerySelector("ul.phones");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(ul);
        var lis = ul.QuerySelectorAll("li");
        Assert.NotEmpty(lis);
        Assert.Equal(expected.Count(), lis.Count());
        foreach (var item in expected)
        {
            var li = lis[index++];
            Assert.NotNull(li);
            var text = li.TextContent;
            Assert.Contains(item.Make, text);
            Assert.Contains(item.Model, text);
            Assert.Equal(2, li.QuerySelectorAll("a").Count());
        }
    }

    [Fact]
    public void CheckProjectType()
    {
        // Arrange
        string path = "../../../../../src/MVCPhones/Program.cs";
        
        // Act
        var result = GetFileContent(path);

        // Assert
        Assert.True(result.fileExists);
        Assert.NotNull(result.content);
        string content = result.content.FilterComments().Condense();
        Assert.Contains(".MapControllerRoute(", content);
        Assert.DoesNotContain(".MapRazorPages(", content);
    }    
}