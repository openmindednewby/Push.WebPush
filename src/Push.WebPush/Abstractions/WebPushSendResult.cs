namespace Push.WebPush.Abstractions;

/// <summary>Per-subscription delivery outcome.</summary>
public sealed class WebPushSubscriptionResult
{
  /// <summary>The subscription endpoint this result refers to.</summary>
  public required string Endpoint { get; init; }

  /// <summary>True if the push service accepted the message.</summary>
  public required bool Ok { get; init; }

  /// <summary>True if the push service reported the subscription is gone (404/410) — prune it.</summary>
  public bool Expired { get; init; }

  /// <summary>Error description when not OK.</summary>
  public string? Error { get; init; }
}

/// <summary>Aggregate result of a web push send.</summary>
public sealed class WebPushSendResult
{
  /// <summary>True if at least one subscription was accepted.</summary>
  public required bool Success { get; init; }

  /// <summary>Per-subscription outcomes (same order as the request).</summary>
  public required IReadOnlyList<WebPushSubscriptionResult> Results { get; init; }

  /// <summary>Endpoints the push service reported as gone (404/410) — the caller should delete these.</summary>
  public IReadOnlyList<string> ExpiredEndpoints =>
    Results.Where(r => r.Expired).Select(r => r.Endpoint).ToList();
}
