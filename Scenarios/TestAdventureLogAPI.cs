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
        private readonly MockQueryHelper _queryHelper;

        public TestAdventureLogAPI(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _queryHelper = new MockQueryHelper();
        }

        [Theory]
        [InlineData("/Home/api/AdventureLogs")]
        public async Task PostAdventureLogTest(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            var itemToSend = new AdventureLog
            {
                UserId = null,
                LogTitle = "Session 22: More Stuff",
                LogBody = "<p><b>The elf jumped over the lazy dwarf.</b></p>"
            };
            var sentContent = JsonConvert.SerializeObject(itemToSend);

            string beforeLogCountQueryResult = _queryHelper.SelectQuery("SELECT TOP 1 HistoricalLogCount FROM HistoricalAdventureLogCounts", "HistoricalLogCount");
            int beforeLogCount = int.Parse(beforeLogCountQueryResult);

            // Act
            var send = await client.PostAsync(url, new StringContent(sentContent, Encoding.UTF8, "application/json"));

            string afterLogCountQueryResult = _queryHelper.SelectQuery("SELECT TOP 1 HistoricalLogCount FROM HistoricalAdventureLogCounts", "HistoricalLogCount");
            int afterLogCount = int.Parse(afterLogCountQueryResult);

            string createdLogTitleQueryResult = _queryHelper.SelectQuery($"SELECT LogTitle FROM AdventureLogs WHERE AdventureLogID = {afterLogCount}", "LogTitle");
            string createdLogBodyQueryResult = _queryHelper.SelectQuery($"SELECT LogBody FROM AdventureLogs WHERE AdventureLogID = {afterLogCount}", "LogBody");

            // Assert
            send.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", send.Content.Headers.ContentType.ToString());
            Assert.Equal(itemToSend.LogTitle, createdLogTitleQueryResult);
            Assert.Equal(itemToSend.LogBody, createdLogBodyQueryResult);
            Assert.NotEqual(beforeLogCount, afterLogCount);

        }

        [Theory]
        [InlineData("/Home/api/AdventureLogs")]
        public async Task GetAdventureLogsTest(string url)
        {
            //Arrange
            var client = _factory.CreateClient();
            //string adventureLog = _queryHelper.SelectQuery("SELECT TOP 1 AdventureLogID FROM AdventureLogs ORDER BY AdventureLogID DESC", "AdventureLogID");

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

        }

        [Theory]
        [InlineData("/Home/api/AdventureLogs")]
        public async Task PutAdventureLogTest(string url)
        {
            //Arrange
            var client = _factory.CreateClient();
            string adventureLogIdResult = _queryHelper.SelectQuery("SELECT TOP 1 AdventureLogID FROM AdventureLogs ORDER BY AdventureLogID DESC", "AdventureLogID");
            int adventureLogId = int.Parse(adventureLogIdResult);
            var itemToSend = new AdventureLog
            {
                AdventureLogID = adventureLogId,
                UserId = null,
                LogTitle = "Session 23: I changed my mind",
                LogBody = "<p><b>The dwarf punched the stupid elf.</b></p>"
            };
            var sentContent = JsonConvert.SerializeObject(itemToSend);

            // Act
            await client.PutAsync(url, new StringContent(sentContent, Encoding.UTF8, "application/json"));

            AdventureLog adventureLog = _queryHelper.SelectAdventureLogQuery(adventureLogId);

            // Assert
            Assert.Equal(itemToSend.LogTitle, adventureLog.LogTitle);
            Assert.Equal(itemToSend.LogBody, adventureLog.LogBody);

        }

        [Theory]
        [InlineData("/Home/api/AdventureLogs")]
        public async Task DeleteAdventureLogTest(string url)
        {
            //Arrange
            var client = _factory.CreateClient();
            string qryResultBeforeDelete = _queryHelper.SelectQuery("SELECT TOP 1 AdventureLogID FROM AdventureLogs ORDER BY AdventureLogID DESC", "AdventureLogID");
            int adventureLogID = int.Parse(qryResultBeforeDelete);

            // Act
            await client.DeleteAsync(url + "/" + adventureLogID);

            // Assert
            string qryResultAfterDelete = _queryHelper.SelectQuery($"SELECT TOP 1 AdventureLogID FROM AdventureLogs WHERE AdventureLogID = {adventureLogID} ORDER BY AdventureLogID DESC", "AdventureLogID");
            Assert.NotStrictEqual(qryResultBeforeDelete, qryResultAfterDelete);

        }
    }
}
