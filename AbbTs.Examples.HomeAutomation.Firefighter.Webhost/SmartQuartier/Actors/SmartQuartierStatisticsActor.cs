using System;
using System.Globalization;
using System.Threading;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

using Akka.Actor;

using Microsoft.Extensions.Logging;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;

public sealed class SmartQuartierStatisticsActor : ReceiveActor
{
  public const string ActorName = "smart-quartier-statistics";
  public const string ActorPath = "/user/" + ActorName;

  private static readonly TimeSpan ClientTimeout = TimeSpan.FromSeconds(5);

  private readonly ISmartQuartierClient _smartQuartierClient;
  private readonly ILogger<SmartQuartierStatisticsActor> _logger;

  private readonly NumericAccumulator _brightness = new();
  private readonly NumericAccumulator _temperature = new();
  private readonly NumericAccumulator _humidity = new();
  private readonly NumericAccumulator _gas = new();

  private int _fireEvents;
  private int _gasEvents;
  private int _motionEvents;
  private int _soundEvents;
  private int _rfidEvents;

  public SmartQuartierStatisticsActor(
      ISmartQuartierClient smartQuartierClient,
      ILogger<SmartQuartierStatisticsActor> logger)
  {
    _smartQuartierClient = smartQuartierClient;
    _logger = logger;

    ReceiveAsync<InitializeSmartQuartierStatistics>(async _ =>
    {
      try
      {
        using var cts = new CancellationTokenSource(ClientTimeout);
        var history = await _smartQuartierClient.GetHistoryDataAsync(cts.Token);

        foreach (var measurement in history.Measurements)
        {
          ObserveMeasurement(measurement);
        }

        foreach (var smartEvent in history.Events)
        {
          ObserveEventType(smartEvent.Type);
        }
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Could not initialize SmartQuartier statistics from history.");
      }
    });

    Receive<ObserveSmartQuartierEnvelope>(message =>
    {
      ObserveFromEnvelope(message.SmartHomeId, message.Envelope);
    });

    Receive<GetSmartQuartierStatistic>(_ =>
    {
      Sender.Tell(BuildResponse());
    });

    Self.Tell(InitializeSmartQuartierStatistics.Instance);
  }

  private void ObserveFromEnvelope(string smartHomeId, SmartHomeGatewayEnvelope envelope)
  {
    if (TryReadMeasurement(smartHomeId, envelope, out var measurement))
    {
      ObserveMeasurement(measurement);
      return;
    }

    if (TryReadEventType(envelope, out var eventType))
    {
      ObserveEventType(eventType);
    }
  }

  private void ObserveMeasurement(SmartQuartierMeasurement measurement)
  {
    _brightness.Observe(measurement.TimeStamp, measurement.BuildingId, measurement.Brightness);
    _temperature.Observe(measurement.TimeStamp, measurement.BuildingId, measurement.Temperature);
    _humidity.Observe(measurement.TimeStamp, measurement.BuildingId, measurement.Humidity);
    _gas.Observe(measurement.TimeStamp, measurement.BuildingId, measurement.Gas);
  }

  private void ObserveEventType(string rawEventType)
  {
    var eventType = rawEventType.Trim().ToLowerInvariant();
    switch (eventType)
    {
      case "fire":
        _fireEvents++;
        break;
      case "gas":
        _gasEvents++;
        break;
      case "motion":
        _motionEvents++;
        break;
      case "sound":
        _soundEvents++;
        break;
      case "rfid":
        _rfidEvents++;
        break;
    }
  }

  private SmartQuartierStatisticResponse BuildResponse()
  {
    return new SmartQuartierStatisticResponse(
        new SmartQuartierMeasurementStatistic(
            _brightness.ToStatistic(),
            _temperature.ToStatistic(),
            _humidity.ToStatistic(),
            _gas.ToStatistic()),
        new SmartQuartierEventStatistic(
            _gasEvents,
            _fireEvents,
            _motionEvents,
            _soundEvents,
            _rfidEvents));
  }

  private static bool TryReadMeasurement(string smartHomeId, SmartHomeGatewayEnvelope envelope, out SmartQuartierMeasurement measurement)
  {
    measurement = default!;

    if (!envelope.MessageType.Equals("send measurement", StringComparison.OrdinalIgnoreCase) ||
        envelope.Payload is null)
    {
      return false;
    }

    var payload = envelope.Payload.Value;
    if (!TryGetInt(payload, "brightness", out var brightness) ||
        !TryGetInt(payload, "temperature", out var temperature) ||
        !TryGetInt(payload, "humidity", out var humidity) ||
        !TryGetInt(payload, "gas", out var gas))
    {
      return false;
    }

    measurement = new SmartQuartierMeasurement(
        envelope.ReceivedAtUtc,
        smartHomeId,
        brightness,
        temperature,
        humidity,
        gas);

    return true;
  }

  private static bool TryReadEventType(SmartHomeGatewayEnvelope envelope, out string eventType)
  {
    eventType = string.Empty;

    if (!envelope.MessageType.Equals("send event", StringComparison.OrdinalIgnoreCase) ||
        envelope.Payload is null)
    {
      return false;
    }

    var payload = envelope.Payload.Value;

    if (TryGetString(payload, "type", out var fromTypeProperty))
    {
      eventType = fromTypeProperty;
      return true;
    }

    if (TryGetString(payload, "eventType", out var fromEventTypeProperty))
    {
      eventType = fromEventTypeProperty;
      return true;
    }

    if (TryGetString(payload, "data", out var fromDataProperty))
    {
      eventType = fromDataProperty;
      return true;
    }

    return false;
  }

  private static bool TryGetInt(System.Text.Json.JsonElement payload, string propertyName, out int value)
  {
    value = 0;
    if (!payload.TryGetProperty(propertyName, out var element))
    {
      return false;
    }

    if (element.ValueKind == System.Text.Json.JsonValueKind.Number && element.TryGetInt32(out var intValue))
    {
      value = intValue;
      return true;
    }

    if (element.ValueKind == System.Text.Json.JsonValueKind.String &&
        int.TryParse(element.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
    {
      value = parsed;
      return true;
    }

    return false;
  }

  private static bool TryGetString(System.Text.Json.JsonElement payload, string propertyName, out string value)
  {
    value = string.Empty;
    if (!payload.TryGetProperty(propertyName, out var element) ||
        element.ValueKind != System.Text.Json.JsonValueKind.String)
    {
      return false;
    }

    var stringValue = element.GetString();
    if (string.IsNullOrWhiteSpace(stringValue))
    {
      return false;
    }

    value = stringValue;
    return true;
  }

  private sealed class NumericAccumulator
  {
    private double _sum;
    private int _count;

    private SmartQuartierStatisticSample? _min;
    private SmartQuartierStatisticSample? _max;

    public void Observe(DateTime timestamp, string buildingId, double value)
    {
      _sum += value;
      _count++;

      var sample = new SmartQuartierStatisticSample(timestamp, buildingId, value);
      _min = _min is null || value < _min.Value ? sample : _min;
      _max = _max is null || value > _max.Value ? sample : _max;
    }

    public SmartQuartierNumericStatistic ToStatistic()
    {
      var min = _min ?? new SmartQuartierStatisticSample(DateTime.MinValue, "-", 0);
      var max = _max ?? new SmartQuartierStatisticSample(DateTime.MinValue, "-", 0);
      var average = _count == 0 ? 0 : _sum / _count;

      return new SmartQuartierNumericStatistic(min, max, average);
    }
  }
}
