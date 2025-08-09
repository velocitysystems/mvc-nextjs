using Microsoft.Extensions.FileProviders;

namespace Webapp.Nextjs.Hosting;

/// <summary>
/// Caches an instance of <see cref="StaticFileOptions"/> so that we can re-use the same <see cref="IFileProvider"/>
/// in <see cref="NextjsEndpointDataSource"/> and <see cref="NextjsStaticHostingExtensions.UseNextjsStaticHosting(IApplicationBuilder)"/>.
/// </summary>
internal class StaticFileOptionsProvider(IWebHostEnvironment env)
{
    public StaticFileOptions StaticFileOptions { get; } = new()
    {
        FileProvider = new PhysicalFileProvider(env.WebRootPath),
    };
}