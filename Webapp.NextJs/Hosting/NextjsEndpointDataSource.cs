using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Webapp.Nextjs.Hosting;

internal class NextjsEndpointDataSource : EndpointDataSource
{
    /// <summary>
    /// Use zero by default so that any conflicts with other default-order ASP .NET Core routes are caught during startup.
    /// </summary>
    private const int DefaultEndpointOrder = 0;

    /// <summary>
    /// Supports dynamic page segments of the form <c>"[param]"</c> and <c>"[...param]"</c>.
    /// This does not currently work with dynamic segments of the form <c>"[[...slug]]"</c>
    /// <see href="https://nextjs.org/docs/routing/dynamic-routes#optional-catch-all-routes">Optional catch all routes</see>.
    /// </summary>
    private static readonly Regex SlugRegex = new(@"^\[([^\[\]]+?)\]$");

    private readonly IEndpointRouteBuilder _endpointRouteBuilder;
    private readonly StaticFileOptionsProvider _staticFileOptionsProvider;
    private readonly Lazy<IReadOnlyList<Endpoint>> _endpoints;

    internal NextjsEndpointDataSource(IEndpointRouteBuilder endpointRouteBuilder, StaticFileOptionsProvider staticFileOptionsProvider)
    {
        _endpointRouteBuilder = endpointRouteBuilder;
        _staticFileOptionsProvider = staticFileOptionsProvider;
        _endpoints = new Lazy<IReadOnlyList<Endpoint>>(BuildEndpoints);
    }

    /// <inheritdoc />
    public override IChangeToken GetChangeToken() => NullChangeToken.Singleton;

    /// <inheritdoc />
    public override IReadOnlyList<Endpoint> Endpoints => _endpoints.Value;

    private IReadOnlyList<Endpoint> BuildEndpoints()
    {
        const string htmlExtension = ".html";
        const string catchAllSlugPrefix = "...";

        var staticFileOptions = _staticFileOptionsProvider.StaticFileOptions;
        if (staticFileOptions.FileProvider is null)
        {
            throw new ArgumentNullException(nameof(staticFileOptions.FileProvider));
        }
        
        var requestDelegate = CreateRequestDelegate(_endpointRouteBuilder, staticFileOptions);
        var endpoints = new List<Endpoint>();
        foreach (var filePath in TraverseFiles(staticFileOptions.FileProvider))
        {
            if (!filePath.EndsWith(htmlExtension))
            {
                continue;
            }

            var fileWithoutHtml = filePath.Substring(0, filePath.Length - htmlExtension.Length);
            var patternSegments = new List<RoutePatternPathSegment>();
            var segments = fileWithoutHtml.Split('/');

            // NOTE: Start at 1 because paths here always have a leading slash
            for (var i = 1; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (i == segments.Length - 1 && segment == "index")
                {
                    // Skip `index` segment, match whatever we got so far.
                    // This is so that e.g. file `/a/b/index.html` is served at path `/a/b`, as desired.
                    // TODO: Should we also serve the same file at `/a/b/index`? Note that `/a/b/index.html` will already work
                    // via the UseStaticFiles middleware added by `NextjsStaticHostingExtensions.UseNextjsStaticHosting`.
                    break;
                }

                var match = SlugRegex.Match(segment);
                if (match.Success)
                {
                    var slugName = match.Groups[1].Value;
                    if (slugName.StartsWith(catchAllSlugPrefix))
                    {
                        // Catch all routes -- see: https://nextjs.org/docs/routing/dynamic-routes#catch-all-routes
                        var parameterName = slugName.Substring(catchAllSlugPrefix.Length);
                        patternSegments.Add(
                            RoutePatternFactory.Segment(RoutePatternFactory.ParameterPart(parameterName, null, RoutePatternParameterKind.CatchAll)));
                    }
                    else
                    {
                        // Dynamic route -- see: https://nextjs.org/docs/routing/dynamic-routes
                        patternSegments.Add(
                            RoutePatternFactory.Segment(RoutePatternFactory.ParameterPart(slugName)));
                    }
                }
                else
                {
                    // Literal match
                    patternSegments.Add(
                        RoutePatternFactory.Segment(RoutePatternFactory.LiteralPart(segment)));
                }
            }
            var endpointBuilder = new RouteEndpointBuilder(requestDelegate, RoutePatternFactory.Pattern(patternSegments), order: DefaultEndpointOrder);

            endpointBuilder.Metadata.Add(new StaticFileEndpointMetadata(filePath));
            endpointBuilder.DisplayName = $"Next.js {filePath}";

            var endpoint = endpointBuilder.Build();
            endpoints.Add(endpoint);
        }

        return endpoints;
    }

    private static IEnumerable<string> TraverseFiles(IFileProvider fileProvider)
    {
        var outstandingPaths = new Stack<string>();
        outstandingPaths.Push("");

        while (outstandingPaths.Count > 0)
        {
            var path = outstandingPaths.Pop();
            foreach (var entry in fileProvider.GetDirectoryContents(path))
            {
                var entryPath = path + "/" + entry.Name;
                if (entry.IsDirectory)
                {
                    outstandingPaths.Push(entryPath);
                }
                else
                {
                    yield return entryPath;
                }
            }
        }
    }

    private static RequestDelegate CreateRequestDelegate(IEndpointRouteBuilder endpoints, StaticFileOptions options)
    {
        var app = endpoints.CreateApplicationBuilder();
        app.Use(next => context =>
        {
            var endpoint = context.GetEndpoint();
            var metadata = endpoint?.Metadata.GetMetadata<StaticFileEndpointMetadata>();
            if (metadata == null)
            {
                throw new InvalidOperationException("Endpoint is missing metadata");
            }

            context.Request.Path = metadata.Path;

            // Set the endpoint to null so the static files middleware will handle the request.
            context.SetEndpoint(null);
            return next(context);
        });

        app.UseStaticFiles(options);
        return app.Build();
    }

    private sealed class StaticFileEndpointMetadata(string path)
    {
        public string Path => path;
    }
}