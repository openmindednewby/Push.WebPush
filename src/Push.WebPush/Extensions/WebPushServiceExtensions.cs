using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Push.WebPush.Abstractions;
using Push.WebPush.Providers.WebPush;

namespace Push.WebPush.Extensions;

/// <summary>DI registration for the VAPID web-push provider.</summary>
public static class WebPushServiceExtensions
{
  /// <summary>Registers <see cref="IWebPushProvider"/> with a typed <see cref="HttpClient"/>.</summary>
  public static IServiceCollection AddWebPush(
    this IServiceCollection services,
    Action<WebPushOptions>? configureOptions = null)
  {
    if (configureOptions != null)
    {
      services.Configure(configureOptions);
    }

    services.AddHttpClient<IWebPushProvider, WebPushProvider>();
    return services;
  }

  /// <summary>Registers <see cref="IWebPushProvider"/>, binding options from the <c>WebPush</c> config section.</summary>
  public static IServiceCollection AddWebPush(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    services.Configure<WebPushOptions>(configuration.GetSection(WebPushOptions.SectionName));
    services.AddHttpClient<IWebPushProvider, WebPushProvider>();
    return services;
  }
}
