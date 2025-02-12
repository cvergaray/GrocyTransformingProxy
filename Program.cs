using AspNetCore.Proxy;
using GrocyTransformingProxy;
using GrocyTransformingProxy.Endpoints.Stock;
using GrocyTransformingProxy.Helpers;
using GrocyTransformingProxy.Strings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddProxies()
	.AddAuthorization()
	.AddHttpClient()
	.AddRequestService();

var app = builder.Build();
app.UseAuthorization();

// Configure the HTTP request pipeline.

var baseUrl = Environment.GetEnvironmentVariable(StringConstants.BaseUrlEnvironmentVariable);
if (string.IsNullOrEmpty(baseUrl))
{
	Console.Out.WriteLine($"{StringConstants.BaseUrlEnvironmentVariable} environment variable must be set.");
	return -1;
}

app.UseProxies(proxies =>
{
	proxies.MapProductAdd();
});

app.RunProxy(proxy => proxy.UseHttp(baseUrl));

app.Run();

return 0;