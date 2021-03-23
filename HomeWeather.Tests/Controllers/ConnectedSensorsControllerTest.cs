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
    public class ConnectedSensorsControllerTest
    {
        private readonly Mock<IPhysSensorInfo> physSensorInfo;
        private readonly ConnectedSensorsController connectedSensorsController;
        private readonly Fixture fixture;

        public ConnectedSensorsControllerTest()
        {
            physSensorInfo = new Mock<IPhysSensorInfo>();
            connectedSensorsController = new ConnectedSensorsController(physSensorInfo.Object);

            fixture = new Fixture();

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async void TestGetConnectedSensors_ReturnSensorObjects()
        {
            // Arrange
            var sensors = fixture.Create<SensorDTO[]>();
            physSensorInfo.Setup(psi => psi.GetSensors()).Returns(Task.FromResult(sensors.AsEnumerable()));

            // Act
            var result = await connectedSensorsController.Get();

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<SensorDTO[]>(okObjResult.Value);
        }

        [Fact]
        public async void TestGetConnectedSensorByID_WithExistsSensor_ReturnSensorObject()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();
            var sensorDTO = fixture.Create<SensorDTO>();
            physSensorInfo.Setup(psi => psi.GetSensorByID(sensor.snID)).Returns(Task.FromResult(sensorDTO));

            // Act
            var result = await connectedSensorsController.Get(sensor.snID);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<SensorDTO>(okObjResult.Value);
        }
    }
}
