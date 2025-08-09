using Webapp.Nextjs;
using Webapp.Nextjs.Hosting;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddNextjsStaticHosting();

builder.Services.AddSingleton<IProxyConfigProvider>(new ProxyConfigProvider(configuration["WebappApiUrl"], configuration["WebappApiKey"]));
builder.Services.AddReverseProxy();
var app = builder.Build();

// Configure reverse proxy.
app.MapReverseProxy();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapNextjsStaticPages();
app.UseNextjsStaticHosting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();