using SecureVault.API.Models;
using SecureVault.Shared.Enums;

namespace SecureVault.API.Services
{
    public class AlarmEvaluationService
    {
        // Thresholds
        private const double TemperatureThreshold = 50.0; // Celsius degrees - high temperature warning
        private const double HumidityThreshold = 80.0; // % - high humidity warning
        private const double CarbonMonoxideThreshold = 35.0; // ppm - dangerous CO levels
        private const double SmokeThreshold = 90.0; // ppm - smoke detection warning

        public bool ShouldTriggerAlarm(Sensor sensor, double value)
        {
            return sensor.Type switch
            {
                SensorType.Temperature => value > TemperatureThreshold,
                SensorType.Humidity => value > HumidityThreshold,
                SensorType.CarbonMonoxide => value > CarbonMonoxideThreshold,
                SensorType.Smoke => value > SmokeThreshold,
                SensorType.Motion => value == 1, // Motion detected, 1 = motion, 0 = no motion
                SensorType.Door => value == 1, // Door opened, 1 = open, 0 = closed
                SensorType.Fire => value == 1, // Fire detected, 1 = fire, 0 = no fire
                SensorType.WaterLeak => value == 1, // Water leak detected, 1 = leak, 0 = no leak
                SensorType.Camera => value == 1, // Camera event detected, 1 = event, 0 = no event
                _ => false
            };
        }

        public string GetAlarmDescription(Sensor sensor, double value)
        {
            return sensor.Type switch
            {
                SensorType.Temperature => $"High temperature detected: {value}°C",
                SensorType.Humidity => $"High humidity detected: {value}%",
                SensorType.CarbonMonoxide => $"Dangerous CO levels detected: {value} ppm",
                SensorType.Smoke => $"Smoke detected: {value} ppm",
                SensorType.Motion => "Motion detected",
                SensorType.Door => "Door opened",
                SensorType.Fire => "Fire detected",
                SensorType.WaterLeak => "Water leak detected",
                SensorType.Camera => "Camera event detected",
                _ => "Unknown alarm"
            };
        }
    }
}
