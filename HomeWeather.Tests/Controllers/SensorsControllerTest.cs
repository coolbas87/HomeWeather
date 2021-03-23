using AutoFixture;
using HomeWeather.Controllers;
using HomeWeather.Data.Entities;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HomeWeather.UnitTests.Controllers
{
    public class SensorsControllerTest
    {
        private readonly Mock<ISensorService> sensorService;
        private readonly SensorsController sensorsController;
        private readonly Fixture fixture;

        public SensorsControllerTest()
        {
            sensorService = new Mock<ISensorService>();
            sensorsController = new SensorsController(sensorService.Object);

            fixture = new Fixture();

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void TestGetSensors_ReturnSensorsObjects()
        {
            // Arrange
            var sensors = fixture.Create<Sensor[]>();
            sensorService.Setup(ss => ss.GetSensors()).Returns(sensors.AsEnumerable());

            // Act
            var result = sensorsController.GetSensors();

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<Sensor[]>(okObjResult.Value);
        }

        [Fact]
        public void TestGetSensors_ReturnNull()
        {
            // Arrange
            var sensors = fixture.Create<Sensor[]>();
            sensorService.Setup(ss => ss.GetSensors()).Returns((IEnumerable<Sensor>)null);

            // Act
            var result = sensorsController.GetSensors();

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.Null(okObjResult.Value);
        }

        [Fact]
        public void TestGetSensorByID_WithExistsSensor_ReturnSensorObject()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();
            sensorService.Setup(ss => ss.GetSensorById(sensor.snID)).Returns(sensor);

            // Act
            var result = sensorsController.GetSensors(sensor.snID);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<Sensor>(okObjResult.Value);
        }

        [Fact]
        public void TestGetSensorByID_WithNotExistsSensor_ReturnSensorObject()
        {
            // Arrange
            var sensorID = fixture.Create<long>();
            sensorService.Setup(ss => ss.GetSensorById(sensorID)).Returns((Sensor)null);

            // Act
            var result = sensorsController.GetSensors(sensorID);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.Null(okObjResult.Value);
        }

        [Fact]
        public void TestAddSensor_ReturnSensorObject()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();
            sensorService.Setup(ss => ss.AddSensor(sensor)).Returns(sensor);

            // Act
            var result = sensorsController.PostSensors(sensor);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<Sensor>(okObjResult.Value);
        }

        [Fact]
        public void TestUpdateSensor_ReturnSensorObject()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();
            sensorService.Setup(ss => ss.UpdateSensor(sensor)).Returns(sensor);

            // Act
            var result = sensorsController.PutSensors(sensor);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<Sensor>(okObjResult.Value);
        }

        [Fact]
        public void TestDeleteSensor_ReturnTrue()
        {
            // Arrange
            var sensorID = fixture.Create<long>();
            sensorService.Setup(ss => ss.DeleteSensor(sensorID)).Returns(true);

            // Act
            var result = sensorsController.DeleteSensors(sensorID);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<bool>(okObjResult.Value);
            Assert.Equal(true, okObjResult.Value);
        }

        [Fact]
        public void TestDeleteSensor_ReturnFalse()
        {
            // Arrange
            var sensorID = fixture.Create<long>();
            sensorService.Setup(ss => ss.DeleteSensor(sensorID)).Returns(false);

            // Act
            var result = sensorsController.DeleteSensors(sensorID);

            // Assert
            Assert.NotNull(result);
            OkObjectResult okObjResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjResult.StatusCode);
            Assert.NotNull(okObjResult.Value);
            Assert.IsType<bool>(okObjResult.Value);
            Assert.Equal(false, okObjResult.Value);
        }
    }
}
