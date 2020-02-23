using Endevrian.Controllers;
using Endevrian.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Endevrian.Tests
{
    public class TestQueryHelper
    {
        private readonly MockQueryHelper _queryHelper;

        public TestQueryHelper()
        {

            _queryHelper = new MockQueryHelper();

        }

        [Fact]
        public void RunQuerySelectTest()
        {
            // Arrange
            string query = "SELECT TOP 1 HistoricalLogCount FROM HistoricalAdventureLogCounts";
            string field = "HistoricalLogCount";

            // Act
            string result = _queryHelper.SelectQuery(query, field);
            bool tryParseResult = int.TryParse(result, out int logCount);

            // Assert
            Assert.True(tryParseResult);

        }

        [Fact]
        public void RunQueryNoResultTest()
        {
            // Arrange
            string query = "SELECT AdventureLogID FROM AdventureLogs WHERE AdventureLogID = 0";
            string field = "AdventureLogID";

            // Act
            string result = _queryHelper.SelectQuery(query, field);
            string expectedResult = "No Results";

            // Assert
            Assert.Equal(result, expectedResult);

        }

        [Fact]
        public void RunQueryUpdateTest()
        {
            // Arrange
            string logCountBeforeQueryResult = _queryHelper.SelectQuery("SELECT HistoricalLogCountTest FROM HistoricalAdventureLogCounts", "HistoricalLogCountTest");
            int logCountBefore = int.Parse(logCountBeforeQueryResult);
            int logCountNew = logCountBefore++;

            string updateQuery = $"UPDATE HistoricalAdventureLogCounts SET HistoricalLogCountTest = {logCountNew}";

            // Act
            _queryHelper.UpdateQuery(updateQuery);

            string logCountAfterQueryResult = _queryHelper.SelectQuery("SELECT HistoricalLogCountTest FROM HistoricalAdventureLogCounts", "HistoricalLogCountTest");
            int logCountAfter = int.Parse(logCountAfterQueryResult);

            // Assert
            Assert.NotEqual(logCountBefore, logCountAfter);

        }
    }
}
