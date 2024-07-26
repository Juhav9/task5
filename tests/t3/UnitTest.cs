using System;
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
    public async void Checkpoint03_CheckFields()
    {
        // Arrange
        var requiredInputs = new List<string>()
        {
            "Make",
            "Model",
            "RAM",
            "PublishDate"
        };

        // Act
        var response = await _client.GetAsync("/phones/add");
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var allInputs = content.QuerySelectorAll("input");
        var dataInputs = allInputs.Where(i => i.GetAttribute("type")?.ToLower() != "submit" && i.GetAttribute("name") != "__RequestVerificationToken");
        var form = content.GetForm();
        var submit = form.GetElement("button");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotEmpty(dataInputs);
        Assert.Equal(requiredInputs.Count, dataInputs.Count());
        var dataInputNames = dataInputs.Select(i => i.GetAttribute("name")).OrderBy(i => i).ToList();
        Assert.Equal(requiredInputs.OrderBy(i => i), dataInputNames);
    }

    [Fact]
    public async void Checkpoint03_Post()
    {
        Random r = new Random();
        // Arrange
        Phone phone = new Phone
        {
            Make = "Tester-" + r.Next(),
            Model = "Automated-" + r.Next(),
            RAM = r.Next(),
            PublishDate = new DateTime(r.Next(2000, DateTime.Now.Year), r.Next(1, 13), 1).AddDays(r.Next(32))
        };

        // Act
        var responseGet = await _client.GetAsync("/phones/add");
        var content = await HtmlHelpers.GetDocumentAsync(responseGet);
        var form = content.GetForm();
        var submit = form.GetSubmitButton();

        var responsePost = await _client.SendAsync(
                form,
                submit,
                new Dictionary<string, string>
                {
                    {"Make", phone.Make},
                    {"Model", phone.Model},
                    {"RAM", phone.RAM.ToString()},
                    {"PublishDate", phone.PublishDate.ToString("O")}
                });
        var phonesPage = await responsePost.Content.ReadAsStringAsync();

        // Assert
        Assert.True(responseGet.IsSuccessStatusCode);
        Assert.True(responsePost.IsSuccessStatusCode);
        Assert.Contains(phone.Make, phonesPage);
        Assert.Contains(phone.Model, phonesPage);
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