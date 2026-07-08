# Push.WebPush

Self-hosted **Web Push (VAPID)** provider for .NET. Sends browser push notifications **directly to the
browser vendor's push gateway** using a VAPID keypair you generate and own — **no Firebase/FCM, no
third-party push service, no account**. Per-subscription delivery results with expired-subscription
(404/410) detection so you can prune dead subscriptions.

Wraps [`Lib.Net.Http.WebPush`](https://www.nuget.org/packages/Lib.Net.Http.WebPush) (VAPID signing +
aes128gcm payload encryption).

## Generate VAPID keys (once)

```bash
npx web-push generate-vapid-keys
```

Put the **private** key in server config (`WebPush:PrivateKey`), hand the **public** key to the browser.

## Register

```csharp
builder.Services.AddWebPush(builder.Configuration);   // binds the "WebPush" section
// or: builder.Services.AddWebPush(o => { o.PublicKey = ...; o.PrivateKey = ...; o.Subject = "mailto:hello@dloizides.com"; });
```

## Send

```csharp
public sealed class MyService(IWebPushProvider push)
{
  public async Task NotifyAsync(IReadOnlyList<WebPushSubscription> subs)
  {
    var result = await push.SendAsync(new WebPushMessage
    {
      Subscriptions = subs,
      Title = "New message",
      Body = "You have a new notification",
      Url = "/inbox",
    });

    foreach (var gone in result.ExpiredEndpoints) { /* delete from your store */ }
  }
}
```

- `SendAsync` never throws for delivery failures — transport/per-subscription errors are on `WebPushSendResult`.
- `WebPushSubscription` is exactly what the browser's `PushManager.subscribe()` returns (`endpoint` + `keys.p256dh` + `keys.auth`).
- `ExpiredEndpoints` lists subscriptions the push service reported as gone (404/410).

## Options

| Option | Default | Notes |
|--------|---------|-------|
| `Subject` | `mailto:hello@dloizides.com` | VAPID subject (mailto/URL identifying the sender). |
| `PublicKey` | _(none)_ | VAPID public key (base64url). Without keys the provider is a no-op. |
| `PrivateKey` | _(none)_ | VAPID private key (base64url) — secret, server-side only. |

## License

MIT
