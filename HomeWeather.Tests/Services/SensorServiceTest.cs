using AutoFixture;
using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Services.Implementation;
using HomeWeather.Domain.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HomeWeather.UnitTests.Services
{
    public class SensorServiceTest
    {
        private readonly ISensorService sensorService;
        private readonly Mock<IRepository<Sensor>> repository;
        private readonly Mock<IUnitOfWork<Sensor>> unitOfWork;
        private readonly Fixture fixture;

        public SensorServiceTest()
        {
            repository = new Mock<IRepository<Sensor>>();
            unitOfWork = new Mock<IUnitOfWork<Sensor>>();

            sensorService = new SensorService(unitOfWork.Object);

            fixture = new Fixture();

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void GetAllSensors_ReturnsSensorsCollection()
        {
            // Arrange
            var sensors = fixture.Build<Sensor>()
                .CreateMany(5)
                .ToList();

            repository.Setup(r => r.Query())
                .Returns(sensors.AsQueryable);
            unitOfWork.Setup(r => r.GetRepository()).Returns(repository.Object);

            // Act
            var result = sensorService.GetSensors();

            // Assert
            Assert.Equal(sensors.AsEnumerable(), result);
        }

        [Fact]
        public void GetSensorById_WhenSensorExists()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();

            repository.Setup(r => r.GetById(sensor.snID))
                .Returns(sensor);
            unitOfWork.Setup(r => r.GetRepository()).
                Returns(repository.Object);

            // Act
            var result = sensorService.GetSensorById(sensor.snID);

            // Assert
            Assert.Equal(sensor, result);
        }

        [Fact]
        public void GetSensorById_WhenSensorNotExists()
        {
            // Arrange
            var sensor = fixture.Create<Sensor>();

            repository.Setup(r => r.GetById(sensor.snID))
                .Returns(sensor);
            unitOfWork.Setup(r => r.GetRepository()).
                Returns(repository.Object);

            // Act
            var result = sensorService.GetSensorById(sensor.snID + 1);

            // Assert
            Assert.Null(result);
        }
    }
}
