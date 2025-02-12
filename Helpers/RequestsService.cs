using Newtonsoft.Json;

namespace GrocyTransformingProxy.Helpers;

public class RequestsService(IHttpClientFactory clientFactory)
{
	public async Task<dynamic> RequestFromEndpoint(string endpoint, string apiKey, HttpMethod? method = null)
	{
		var requestMessage = new HttpRequestMessage(method ?? HttpMethod.Get, endpoint.RelativeToFullPath());
		requestMessage.Headers.Add(StringConstants.API_KEY_HEADER, apiKey);
		var client = clientFactory.CreateClient();
		var response = await client.SendAsync(requestMessage);
		response.EnsureSuccessStatusCode();
		var dynamicContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
		if (dynamicContent == null)
			throw new NullReferenceException(nameof(dynamicContent));
		return dynamicContent;
	}

	public Task<dynamic> RequestFromGrocy(HttpRequestMessage request, string endpoint, HttpMethod? method = null)
	{
		var apiKey = request.Headers.First(header => header.Key == StringConstants.API_KEY_HEADER).Value.ToList();
		if (apiKey == null || apiKey.Count == 0)
			throw new ArgumentException("API KEY Not Provided", nameof(request));
		return RequestFromEndpoint(endpoint, apiKey.First(), method);
	}
}

public static class RequestServiceExtensions
{
	public static IServiceCollection AddRequestService(this IServiceCollection services)
	{
		services.AddSingleton<RequestsService>();
		return services;
	}

}