using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

using Akka.Actor;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;

public sealed class SmartHomeManagerActor : ReceiveActor
{
  public const string ActorName = "smart-home-manager";
  public const string ActorPath = "/user/" + ActorName;

  private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(5);

  private readonly Dictionary<string, IActorRef> _smartHomes = new(
    StringComparer.OrdinalIgnoreCase
  );
  private readonly HashSet<string> _allocatedActorNames = new(StringComparer.OrdinalIgnoreCase);

  public SmartHomeManagerActor()
  {
    Receive<ConnectSmartHomeSession>(message =>
    {
      var actor = GetOrCreateActor(message.Registration);
      actor.Tell(new AttachDeviceSocket(message.WebSocket));
      Sender.Tell(new Status.Success(message.Registration.BuildingId));
    });

    Receive<RecordSmartHomeEnvelope>(message =>
    {
      if (_smartHomes.TryGetValue(message.SmartHomeId, out var actor))
      {
        actor.Forward(message);
      }
    });

    Receive<DisconnectSmartHomeSession>(message =>
    {
      if (_smartHomes.TryGetValue(message.SmartHomeId, out var actor))
      {
        actor.Tell(new MarkDeviceDisconnected(message.WebSocket));
      }
    });

    ReceiveAsync<SendSmartHomeDashboardCommand>(async message =>
    {
      var replyTo = Sender;

      if (!_smartHomes.TryGetValue(message.SmartHomeId, out var actor))
      {
        replyTo.Tell(
          new SmartHomeCommandResult(false, $"Unknown smart-home '{message.SmartHomeId}'.")
        );
        return;
      }

      var result = await actor.Ask<SmartHomeCommandResult>(message, AskTimeout);
      replyTo.Tell(result);
    });

    ReceiveAsync<GetSmartHomes>(async _ =>
    {
      var replyTo = Sender;
      var tasks = _smartHomes.Values.Select(actor =>
        actor.Ask<SmartHomeSummary>(new GetSmartHomeSummary(), AskTimeout)
      );

      var summaries = await Task.WhenAll(tasks);
      replyTo.Tell(
        summaries.OrderBy(summary => summary.Id, StringComparer.OrdinalIgnoreCase).ToArray()
      );
    });

    ReceiveAsync<GetSmartHomeById>(async message =>
    {
      var replyTo = Sender;

      if (!_smartHomes.TryGetValue(message.SmartHomeId, out var actor))
      {
        replyTo.Tell(null);
        return;
      }

      var details = await actor.Ask<SmartHomeDetails>(new GetSmartHomeDetails(), AskTimeout);
      replyTo.Tell(details);
    });
  }

  private IActorRef GetOrCreateActor(SmartHomeRegistration registration)
  {
    if (_smartHomes.TryGetValue(registration.BuildingId, out var existingActor))
    {
      return existingActor;
    }

    var actorName = AllocateActorName(registration.BuildingId);

    var actor = Context.ActorOf(Props.Create(() => new SmartHomeActor(registration)), actorName);

    _smartHomes[registration.BuildingId] = actor;
    return actor;
  }

  private string AllocateActorName(string buildingId)
  {
    var normalized = NormalizeActorNameSegment(buildingId);
    var baseName = $"smart-home-{normalized}";
    var candidate = baseName;

    while (!_allocatedActorNames.Add(candidate))
    {
      candidate = $"{baseName}-{Guid.NewGuid():N}";
    }

    return candidate;
  }

  private static string NormalizeActorNameSegment(string value)
  {
    var buffer = new StringBuilder(value.Length);

    foreach (var character in value)
    {
      if (char.IsLetterOrDigit(character) || character is '-' or '_')
      {
        buffer.Append(character);
      }
      else
      {
        buffer.Append('-');
      }
    }

    var normalized = buffer.ToString().Trim('-');
    return string.IsNullOrWhiteSpace(normalized) ? "unknown" : normalized;
  }
}
