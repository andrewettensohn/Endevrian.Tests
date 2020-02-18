using Endevrian.Data;
using Endevrian.Models;
using Endevrian.Utility;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Endevrian.Tests
{
    public class TestAdventureLogAPI
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public TestAdventureLogAPI(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/Home/api/AdventureLogs")]
        public async Task PostAdventureLogTest(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var itemToSend = new AdventureLog
            {
                UserID = null,
                LogTitle = "Session 22: More Stuff",
                LogBody = "The elf jumped over the lazy dwarf."
            };
            var sentContent = JsonConvert.SerializeObject(itemToSend);

            // Act
            var send = await client.PostAsync(url, new StringContent(sentContent, Encoding.UTF8, "application/json"));

            // Assert
            send.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", send.Content.Headers.ContentType.ToString());

        }

        [Theory]
        [InlineData("/Home/api/AdventureLogs")]
        public async Task GetAdventureLogsTest(string url)
        {
            //Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

        }

        //[Theory]
        //[InlineData("/Home/api/AdventureLogs")]
        //public async Task GetAdventureLogTest(string url)
        //{
        //    //Arrange
        //    var client = _factory.CreateClient();

        //    // Act
        //    var response = await client.GetAsync(url + "/" + expectedLogID);

        //    // Assert
        //    response.EnsureSuccessStatusCode(); // Status Code 200-299
        //    Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

        //}

        [Theory]
        [InlineData("/Home/api/AdventureLogs")]
        public async Task DeleteAdventureLogTest(string url)
        {
            //Arrange
            var client = _factory.CreateClient();
            string queryAdventureLogID = QueryHelper.RunQuery("SELECT TOP 1 AdventureLogID FROM AdventureLogs", "AdventureLogID");
            int adventureLogID = int.Parse(queryAdventureLogID);

            // Act
            await client.DeleteAsync(url + "/" + adventureLogID);

            // Assert
            string queryAdventureLogExist = QueryHelper.RunQuery($"SELECT TOP 1 AdventureLogID FROM AdventureLogs WHERE AdventureLogID = {adventureLogID}", "AdventureLogID");
            Assert.NotStrictEqual(queryAdventureLogExist, queryAdventureLogID);

        }
    }
}
