namespace Push.WebPush.Providers.WebPush;

/// <summary>Configuration for the VAPID web-push provider.</summary>
public sealed class WebPushOptions
{
  /// <summary>Config section name for binding.</summary>
  public const string SectionName = "WebPush";

  /// <summary>VAPID subject — a <c>mailto:</c> or site URL identifying the sender.</summary>
  public string Subject { get; set; } = "mailto:hello@dloizides.com";

  /// <summary>VAPID public key (base64url) — also handed to the browser as the application server key.</summary>
  public string? PublicKey { get; set; }

  /// <summary>VAPID private key (base64url) — secret; keep server-side only.</summary>
  public string? PrivateKey { get; set; }
}
