namespace Push.WebPush.Abstractions;

/// <summary>
/// A browser Web Push subscription, as produced by <c>PushManager.subscribe()</c> on the client.
/// Store these per user/device and pass them to <see cref="IWebPushProvider"/>.
/// </summary>
public sealed class WebPushSubscription
{
  /// <summary>The push service endpoint URL the browser returned.</summary>
  public required string Endpoint { get; init; }

  /// <summary>The client public key (subscription <c>keys.p256dh</c>), base64url.</summary>
  public required string P256dh { get; init; }

  /// <summary>The client auth secret (subscription <c>keys.auth</c>), base64url.</summary>
  public required string Auth { get; init; }
}
