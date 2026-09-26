using SecureVault.API.Models;
using SecureVault.API.Services;
using SecureVault.Shared.Enums;
using Xunit;

namespace SecureVault.Tests
{
    /// <summary>
    /// Unit tests for <see cref="AlarmEvaluationService"/>, verifying
    /// that alarms are triggered correctly based on sensor type and
    /// reading value.
    /// </summary>
    public class AlarmEvaluationServiceTests
    {
        private readonly AlarmEvaluationService _service = new();

        // Verifies that a temperature reading above the threshold (50°C) triggers an alarm
        [Fact]
        public void ShouldTriggerAlarm_TemperatureAboveThreshold_ReturnsTrue()
        {
            var sensor = new Sensor { Type = SensorType.Temperature };

            var result = _service.ShouldTriggerAlarm(sensor, 65.0);

            Assert.True(result);
        }

        // Verifies that a temperature reading below the threshold does not trigger an alarm
        [Fact]
        public void ShouldTriggerAlarm_TemperatureBelowThreshold_ReturnsFalse()
        {
            var sensor = new Sensor { Type = SensorType.Temperature };

            var result = _service.ShouldTriggerAlarm(sensor, 22.0);

            Assert.False(result);
        }

        // Verifies that a carbon monoxide reading above the threshold (35 ppm) triggers an alarm
        [Fact]
        public void ShouldTriggerAlarm_CarbonMonoxideAboveThreshold_ReturnsTrue()
        {
            var sensor = new Sensor { Type = SensorType.CarbonMonoxide };

            var result = _service.ShouldTriggerAlarm(sensor, 40.0);

            Assert.True(result);
        }

        // Verifies that a motion sensor reporting motion (value = 1) triggers an alarm
        [Fact]
        public void ShouldTriggerAlarm_MotionDetected_ReturnsTrue()
        {
            var sensor = new Sensor { Type = SensorType.Motion };

            var result = _service.ShouldTriggerAlarm(sensor, 1);

            Assert.True(result);
        }

        // Verifies that a motion sensor reporting no motion (value = 0) does NOT trigger an alarm
        [Fact]
        public void ShouldTriggerAlarm_MotionNotDetected_ReturnsFalse()
        {
            var sensor = new Sensor { Type = SensorType.Motion };

            var result = _service.ShouldTriggerAlarm(sensor, 0);

            Assert.False(result);
        }

        // Verifies that the alarm description for a temperature sensor includes the correct value and unit
        [Fact]
        public void GetAlarmDescription_Temperature_ReturnsExpectedMessage()
        {
            var sensor = new Sensor { Type = SensorType.Temperature };

            var result = _service.GetAlarmDescription(sensor, 75.0);

            Assert.Equal("High temperature detected: 75°C", result);
        }
    }
}