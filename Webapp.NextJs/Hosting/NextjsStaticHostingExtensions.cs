namespace Webapp.Nextjs.Hosting;

/// <summary>
/// Extension method to add support for hosting a Next.js statically generated client-side app on ASP .NET Core.
/// </summary>
public static class NextjsStaticHostingExtensions
{
    /// <summary>
    /// Adds services required for hosting a static, statically generated Next.js client-side application on ASP .NET Core.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    public static void AddNextjsStaticHosting(this IServiceCollection services)
    {
        services.AddSingleton<StaticFileOptionsProvider>();
    }

    /// <summary>
    /// Registers endpoints for Next.js pages found in wwwroot.
    /// This ensures that the correct static Next.js pages will be served at the right paths.
    /// <para>
    ///   For example, say your client application is composed of the following files:
    ///   <list type="bullet">
    ///     <item><c>/post/create.html</c></item>
    ///     <item><c>/post/[pid].html</c></item>
    ///     <item><c>/post/[...slug].html</c></item>
    ///   </list>
    /// </para>
    /// <para>
    ///   The following routes will be created accordingly:
    ///   <list type="bullet">
    ///     <item><c>post/create</c>, serving file <c>/post/create.html</c></item>
    ///     <item><c>post/{pid}</c>, serving file <c>/post/[pid].html</c></item>
    ///     <item><c>post/{*slug}</c>, serving file <c>/post/[...slug].html</c></item>
    ///   </list>
    /// </para>
    /// <para>
    /// ASP .NET Core Endpoint Routing built-in route precedence rules ensure the same semantics as Next.js expects will apply.
    /// E.g., <c>post/create</c> has higher precedence than <c>post/{pid}</c>, which in turn has higher precedence than <c>post/{*slug}</c>.
    /// </para>
    /// </summary>
    public static void MapNextjsStaticPages(this IEndpointRouteBuilder endpoints)
    {
        var env = endpoints.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            return;
        }

        var staticFileOptionsProvider = endpoints.ServiceProvider.GetRequiredService<StaticFileOptionsProvider>();
        var dataSource = new NextjsEndpointDataSource(endpoints, staticFileOptionsProvider);
        endpoints.DataSources.Add(dataSource);
    }

    /// <summary>
    /// Registers a static files middleware that will serve files from wwwroot.
    /// This is required to serve non-html files including JavaScript, CSS, and any other artifacts produced by Next.js export outputs.
    /// </summary>
    public static void UseNextjsStaticHosting(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            return;
        }

        var staticFileOptionsProvider = app.ApplicationServices.GetRequiredService<StaticFileOptionsProvider>();
        app.UseStaticFiles(staticFileOptionsProvider.StaticFileOptions);
    }
}