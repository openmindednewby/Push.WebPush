using System.Net;
using System.Text.Json;

using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Push.WebPush.Abstractions;

namespace Push.WebPush.Providers.WebPush;

/// <summary>
/// <see cref="IWebPushProvider"/> backed by VAPID web push (Lib.Net.Http.WebPush). Sends directly
/// to each browser's push gateway — no Firebase/FCM, no third-party service. Detects expired
/// subscriptions (404/410) so the caller can prune them.
/// </summary>
public sealed class WebPushProvider : IWebPushProvider
{
  private readonly PushServiceClient _client;
  private readonly WebPushOptions _options;
  private readonly ILogger<WebPushProvider> _logger;
  private readonly bool _isConfigured;

  public WebPushProvider(
    HttpClient httpClient,
    IOptions<WebPushOptions> options,
    ILogger<WebPushProvider> logger)
  {
    _options = options.Value;
    _logger = logger;
    _client = new PushServiceClient(httpClient);

    _isConfigured = !string.IsNullOrWhiteSpace(_options.PublicKey) && !string.IsNullOrWhiteSpace(_options.PrivateKey);
    if (_isConfigured)
    {
      _client.DefaultAuthentication = new VapidAuthentication(_options.PublicKey!, _options.PrivateKey!)
      {
        Subject = _options.Subject,
      };
    }
    else
    {
      _logger.LogWarning("WebPush is not configured (missing VAPID keys); web push notifications will not be sent.");
    }
  }

  /// <inheritdoc />
  public async Task<WebPushSendResult> SendAsync(WebPushMessage message, CancellationToken cancellationToken = default)
  {
    if (!_isConfigured)
    {
      var skipped = message.Subscriptions
        .Select(s => new WebPushSubscriptionResult { Endpoint = s.Endpoint, Ok = false, Error = "NotConfigured" })
        .ToList();
      return new WebPushSendResult { Success = false, Results = skipped };
    }

    var payload = BuildPayload(message);
    var results = new List<WebPushSubscriptionResult>(message.Subscriptions.Count);

    foreach (var subscription in message.Subscriptions)
    {
      var push = new PushSubscription { Endpoint = subscription.Endpoint };
      push.SetKey(PushEncryptionKeyName.P256DH, subscription.P256dh);
      push.SetKey(PushEncryptionKeyName.Auth, subscription.Auth);

      try
      {
        await _client.RequestPushMessageDeliveryAsync(push, new PushMessage(payload), cancellationToken);
        results.Add(new WebPushSubscriptionResult { Endpoint = subscription.Endpoint, Ok = true });
      }
      catch (PushServiceClientException ex)
      {
        var expired = ex.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone;
        if (!expired)
        {
          _logger.LogError(ex, "Web push delivery failed ({Status}) for endpoint", (int)ex.StatusCode);
        }
        results.Add(new WebPushSubscriptionResult
        {
          Endpoint = subscription.Endpoint,
          Ok = false,
          Expired = expired,
          Error = expired ? "Expired" : $"Http{(int)ex.StatusCode}",
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Web push transport error for endpoint");
        results.Add(new WebPushSubscriptionResult { Endpoint = subscription.Endpoint, Ok = false, Error = "TransportError" });
      }
    }

    return new WebPushSendResult { Success = results.Any(r => r.Ok), Results = results };
  }

  private static string BuildPayload(WebPushMessage message)
  {
    var payload = new Dictionary<string, object?>
    {
      ["title"] = message.Title,
      ["body"] = message.Body,
    };
    if (message.Icon is not null) payload["icon"] = message.Icon;
    if (message.Url is not null) payload["url"] = message.Url;
    if (message.Data is not null) payload["data"] = message.Data;
    return JsonSerializer.Serialize(payload);
  }
}
