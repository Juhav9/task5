using System;
using System.Linq;
using System.Net.Http;
using MVCPhones.Data;
using Savonia.xUnit.Helpers;
using Xunit;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using Savonia.xUnit.Helpers.Infrastructure;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>

{
    private readonly HttpClient _client;
    private readonly PhonesContext _db;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture) : base(new string[] {"Phones.db"}, testOutputHelper)
    {
        _client = fixture.CreateClient();
        WriteLine(fixture.HostUrl);

        string connectionstring = "Data Source=Phones.db";

        var optionsBuilder = new DbContextOptionsBuilder<PhonesContext>();
        optionsBuilder.UseSqlite(connectionstring);

        _db = new PhonesContext(optionsBuilder.Options);
    }

    [Fact]
    public async void Checkpoint05_CheckFields()
    {
        // Arrange
        Random r = new Random();
        int skip = r.Next(0, _db.Phones.Count());
        Phone phone = await _db.Phones.Skip(skip).Take(1).FirstAsync();

        // Act
        var response = await _client.GetAsync("/phones/delete/" + phone.Id.ToString());
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var form = content.GetForm();
        var submit = form.GetElement("button");

        var deletePage = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains(phone.Make, deletePage);
        Assert.Contains(phone.Model, deletePage);
    }

    [Fact]
    public async void Checkpoint05_Post()
    {
        // Arrange
        Random r = new Random();
        int skip = r.Next(0, _db.Phones.Count());
        Phone phone = await _db.Phones.Skip(skip).Take(1).FirstAsync();

        // Act
        var responseGet = await _client.GetAsync("/phones/delete/" + phone.Id.ToString());
        var content = await HtmlHelpers.GetDocumentAsync(responseGet);
        var form = content.GetForm();
        var submit = form.GetSubmitButton();

        var responsePost = await _client.SendAsync(form, submit);
        var phonesPage = await responsePost.Content.ReadAsStringAsync();

        // Assert
        Assert.True(responseGet.IsSuccessStatusCode);
        Assert.True(responsePost.IsSuccessStatusCode);
        Assert.DoesNotContain($"phones/edit/{phone.Id}", phonesPage, StringComparison.OrdinalIgnoreCase);
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