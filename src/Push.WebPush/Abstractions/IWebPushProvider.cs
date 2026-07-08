namespace Push.WebPush.Abstractions;

/// <summary>Sends Web Push notifications to browser subscriptions (VAPID, self-hosted).</summary>
public interface IWebPushProvider
{
  /// <summary>
  /// Sends a message to its target subscriptions. Never throws for delivery failures —
  /// transport/per-subscription errors (incl. expired 404/410) are on the returned result.
  /// </summary>
  Task<WebPushSendResult> SendAsync(WebPushMessage message, CancellationToken cancellationToken = default);
}
