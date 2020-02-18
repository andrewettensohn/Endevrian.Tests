using Endevrian.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Endevrian.Tests
{
    public class TestQueryHelper
    {

        [Fact]
        public void RunQuerySelectTest()
        {
            // Arrange
            string query = "SELECT TOP 1 HistoricalLogCount FROM HistoricalAdventureLogCounts";
            string field = "HistoricalLogCount";

            // Act
            string result = QueryHelper.RunQuery(query, field);
            bool tryParseResult = int.TryParse(result, out int logCount);

            // Assert
            string msg = $"Expected: True Actual: {tryParseResult}";
            Assert.True(tryParseResult, msg);

        }

        [Fact]
        public void RunQueryNoResultTest()
        {
            // Arrange
            string query = "SELECT AdventureLogID FROM AdventureLogs WHERE AdventureLogID = 0";
            string field = "AdventureLogID";

            // Act
            string result = QueryHelper.RunQuery(query, field);
            string expectedResult = "No Results";

            // Assert
            Assert.Equal(result, expectedResult);

        }
    }
}
