using Moq;
using SpreadsheetUtility.Library.Calculators;
using SpreadsheetUtility.Library.Models;
using SpreadsheetUtility.Library.Providers;

namespace SpreadsheetUtility.Tests.Calculators
{
    public class DateCalculatorTests
    {
        private readonly Mock<IHolidayProvider> _holidayProviderMock;
        private readonly DateCalculator _dateCalculator;

        public DateCalculatorTests()
        {
            _holidayProviderMock = new Mock<IHolidayProvider>();
            _holidayProviderMock.Setup(h => h.LoadHolidaysFromConfigurationFile())
                .Returns(new List<Holiday>
                {
                    new Holiday { Date = new DateTime(2025, 12, 25), HolidayDescription = "Christmas" },
                    new Holiday { Date = new DateTime(2025, 12, 24), HolidayDescription = "Christmas eve" },
                    new Holiday { Date = new DateTime(2026, 1, 1), HolidayDescription = "New Year" }
                });

            _dateCalculator = new DateCalculator(_holidayProviderMock.Object);
        }

        [Theory]
        [InlineData("2025-12-20", "2025-12-22")] // Start on Saturday, skips weekend
        [InlineData("2025-12-21", "2025-12-22")] // Start on Sunday, skips weekend
        [InlineData("2025-12-25", "2025-12-26")] // Start on Christmas, skips holiday
        [InlineData("2025-12-26", "2025-12-26")] // Start on a working day
        public void GetNextWorkingDay_ShouldReturnNextWorkingDay_WhenStartDateIsWeekend(string startDateString, string expectedDateString)
        {
            // Arrange
            var startDate = DateTime.Parse(startDateString);
            var expectedDate = DateTime.Parse(expectedDateString);

            // Act
            var result = _dateCalculator.GetNextWorkingDay(startDate);

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Theory]
        [InlineData("2025-12-22", 3, "2025-12-26")] // Start on Monday, skips Christmas
        [InlineData("2025-12-23", 2, "2025-12-26")] // Start on Tuesday, skips Christmas
        [InlineData("2025-12-25", 1, "2025-12-26")] // Start on Christmas, skips holiday
        [InlineData("2025-12-26", 2, "2025-12-29")] // Start on a working day
        public void CalculateEndDate_ShouldReturnCorrectEndDate_WhenGivenWorkDays(string startDateString, double workDays, string expectedDateString)
        {
            // Arrange
            var startDate = DateTime.Parse(startDateString);
            var expectedDate = DateTime.Parse(expectedDateString);

            // Act
            var result = _dateCalculator.CalculateEndDate(startDate, workDays, null);

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public void CalculateWorkDays_ShouldReturnCorrectCount_WhenGivenDateRange()
        {
            // Arrange
            var startDate = new DateTime(2025, 12, 22); // Monday
            var endDate = new DateTime(2025, 12, 29); // Monday

            // Act
            var result = _dateCalculator.CalculateWorkDays(startDate, endDate, null);

            // Assert
            Assert.Equal(4, result); // Excludes weekend and Christmas
        }

        [Fact]
        public void CalculateVacationDays_ShouldReturnCorrectCount_WhenVacationsAreProvided()
        {
            // Arrange
            var startDate = new DateTime(2025, 12, 15);
            var endDate = new DateTime(2025, 12, 22);
            var vacations = new List<(DateTime Start, DateTime End)?>
            {
                (new DateTime(2025, 12, 18), new DateTime(2025, 12, 19))
            };

            // Act
            var result = _dateCalculator.CalculateVacationDays(startDate, endDate, vacations);

            // Assert
            Assert.Equal(2, result); // Includes only vacation days
        }

        [Fact]
        public void CalculateNonWorkingDays_ShouldReturnCorrectCount_WhenGivenDateRange()
        {
            // Arrange
            var startDate = new DateTime(2025, 12, 22);
            var endDate = new DateTime(2025, 12, 29);
            var vacations = new List<(DateTime Start, DateTime End)?>
            {
                (new DateTime(2025, 12, 26), new DateTime(2025, 12, 27))
            };

            // Act
            var result = _dateCalculator.CalculateNonWorkingDays(startDate, endDate, vacations);

            // Assert
            Assert.Equal(5, result); // Includes weekend, Christmas, and vacation days
        }

        [Fact]
        public void AddObserver_ShouldNotifyObserver_WhenHolidayOccurs()
        {
            // Arrange
            var observerMock = new Mock<IObserver<Holiday>>();
            _dateCalculator.AddObserver(observerMock.Object);

            // Act
            _dateCalculator.GetNextWorkingDay(new DateTime(2025, 12, 25)); // Christmas

            // Assert
            observerMock.Verify(o => o.OnNext(It.Is<Holiday>(h => h.Date == new DateTime(2025, 12, 25))), Times.Once);
        }

        [Fact]
        public void RemoveObserver_ShouldNotNotifyObserver_AfterRemoval()
        {
            // Arrange
            var observerMock = new Mock<IObserver<Holiday>>();
            _dateCalculator.AddObserver(observerMock.Object);
            _dateCalculator.RemoveObserver(observerMock.Object);

            // Act
            _dateCalculator.GetNextWorkingDay(new DateTime(2025, 12, 25)); // Christmas

            // Assert
            observerMock.Verify(o => o.OnNext(It.IsAny<Holiday>()), Times.Never);
        }
    }
}
