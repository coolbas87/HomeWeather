using AutoFixture;
using HomeWeather.Controllers;
using HomeWeather.Data.Entities;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HomeWeather.UnitTests.Controllers
{
    public class TempHistoryControllerTest
    {
        private readonly Mock<ITempHistoryService> tempHistoryService;
        private readonly TempHistoryController tempHistoryController;
        private readonly Fixture fixture;

        public TempHistoryControllerTest()
        {
            tempHistoryService = new Mock<ITempHistoryService>();
            tempHistoryController = new TempHistoryController(tempHistoryService.Object);

            fixture = new Fixture();

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void TestGetTempHistory_ReturnTempHistoryObjects()
        {
            // Arrange
            var tempHistory = fixture.Create<TempHistory[]>();
            var dateFrom = DateTime.Now;
            var dateTo = DateTime.Now.AddDays(7);
            tempHistoryService.Setup(ths => ths.GetTempHistory(dateFrom, dateTo)).Returns(tempHistory.AsEnumerable());

            // Act
            var result = tempHistoryController.GetTempHistory(dateFrom, dateTo);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<TempHistory[]>(okObjResult.Value);
        }

        [Fact]
        public void TestGetTempHistory_ReturnNull()
        {
            // Arrange
            var dateFrom = DateTime.Now;
            var dateTo = DateTime.Now.AddDays(7);
            tempHistoryService.Setup(ths => ths.GetTempHistory(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns((IEnumerable<TempHistory>)null);

            // Act
            var result = tempHistoryController.GetTempHistory(dateFrom, dateTo);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.Null(okObjResult.Value);
        }

        [Fact]
        public void TestGetTempHistoryByID_WithExistsSensor_ReturnTempHistoryObjects()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();
            var tempHistory = fixture.Create<TempHistory[]>();
            var dateFrom = DateTime.Now;
            var dateTo = DateTime.Now.AddDays(7);
            tempHistoryService.Setup(ths => ths.GetTempHistoryBySensor(sensor.snID, dateFrom, dateTo)).Returns(tempHistory.AsEnumerable());

            // Act
            var result = tempHistoryController.GetTempHistory(sensor.snID, dateFrom, dateTo);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<TempHistory[]>(okObjResult.Value);
        }

        [Fact]
        public void TestGetTempHistoryByID_WithExistsSensor_ReturnNull()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();
            var tempHistory = fixture.Create<TempHistory[]>();
            var dateFrom = DateTime.Now;
            var dateTo = DateTime.Now.AddDays(7);
            tempHistoryService.Setup(ths => ths.GetTempHistoryBySensor(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns((IEnumerable<TempHistory>)null);

            // Act
            var result = tempHistoryController.GetTempHistory(sensor.snID, dateFrom, dateTo);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.Null(okObjResult.Value);
        }
    }
}
