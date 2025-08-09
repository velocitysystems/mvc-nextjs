using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Transforms;

namespace Webapp.Nextjs;

/// <inheritdoc />
public class ProxyConfigProvider : IProxyConfigProvider
{
    private const string ApiKeyHeader = "Api-Key-Header";
    private readonly CustomMemoryConfig _config;
    
    public ProxyConfigProvider(string? apiUrl, string? apiKey)
    {
        apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
        apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

        var routeConfig = new RouteConfig
        {
            RouteId = "route1",
            ClusterId = "cluster1",
            Match = new RouteMatch
            {
                Path = "/client/{**catch-all}"
            },
            Transforms =
            [
                new Dictionary<string, string>
                {
                    { "PathRemovePrefix", "/client" }
                }
            ]
        };

        var routes = new[] { routeConfig.WithTransformRequestHeader(ApiKeyHeader, apiKey) };
        var clusters = new[]
        {
            new ClusterConfig
            {
                ClusterId = "cluster1",
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "destination1", new DestinationConfig { Address = apiUrl } }
                }
            }
        };

        _config = new CustomMemoryConfig(routes, clusters);
    }

    /// <inheritdoc />
    public IProxyConfig GetConfig() => _config;
    
    /// <inheritdoc />
    private sealed class CustomMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters) : IProxyConfig
    {
        /// <inheritdoc />
        public IReadOnlyList<RouteConfig> Routes { get; } = routes;

        /// <inheritdoc />
        public IReadOnlyList<ClusterConfig> Clusters { get; } = clusters;

        /// <inheritdoc />
        public IChangeToken ChangeToken { get; } = new CancellationChangeToken(CancellationToken.None);
    }
}