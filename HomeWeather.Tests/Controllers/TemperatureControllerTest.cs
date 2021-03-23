using AutoFixture;
using HomeWeather.Controllers;
using HomeWeather.Data.Entities;
using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HomeWeather.UnitTests.Controllers
{
    public class TemperatureControllerTest
    {
        private readonly Mock<ISensorTempReader> tempReader;
        private readonly TemperatureController temperatureController;
        private readonly Fixture fixture;

        public TemperatureControllerTest()
        {
            tempReader = new Mock<ISensorTempReader>();
            temperatureController = new TemperatureController(tempReader.Object);

            fixture = new Fixture();

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async void TestGetTemperature_ReturnTempObjects()
        {
            // Arrange
            var temps = fixture.Create<TempDTO[]>();
            tempReader.Setup(tr => tr.GetTempAllSensors()).Returns(Task.FromResult(temps.AsEnumerable()));

            // Act
            var result = await temperatureController.Get();

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<TempDTO[]>(okObjResult.Value);
        }

        [Fact]
        public async void TestGetTemperatureByID_WithExistsSensor_ReturnTempObject()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();
            var temp = fixture.Create<TempDTO>();
            tempReader.Setup(tr => tr.GetTempBySensor(sensor.snID)).Returns(Task.FromResult(temp));

            // Act
            var result = await temperatureController.Get(sensor.snID);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<TempDTO>(okObjResult.Value);
        }
    }
}
