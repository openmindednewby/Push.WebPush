namespace Push.WebPush.Abstractions;

/// <summary>A web push notification to deliver to one or more browser subscriptions.</summary>
public sealed class WebPushMessage
{
  /// <summary>The target browser subscriptions.</summary>
  public required IReadOnlyList<WebPushSubscription> Subscriptions { get; init; }

  /// <summary>Notification title.</summary>
  public required string Title { get; init; }

  /// <summary>Notification body.</summary>
  public required string Body { get; init; }

  /// <summary>Optional structured data delivered to the service worker's push handler.</summary>
  public IReadOnlyDictionary<string, string>? Data { get; init; }

  /// <summary>Optional icon URL.</summary>
  public string? Icon { get; init; }

  /// <summary>Optional URL to open when the notification is clicked.</summary>
  public string? Url { get; init; }
}
