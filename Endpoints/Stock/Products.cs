using System.Net.Http.Headers;
using AspNetCore.Proxy.Builders;
using GrocyTransformingProxy.Helpers;
using Newtonsoft.Json;

namespace GrocyTransformingProxy.Endpoints.Stock;

public static class Products
{
	public static void MapAdd(IProxiesBuilder proxies, string baseUrl)
	{
		proxies.Map("api/stock/products/{productId}/add", proxy => proxy.UseHttp(
			(_, args) => $"{baseUrl}/api/stock/products/{args["productId"]}/add",
			options => options.WithBeforeSend(TransformAdd)
		));
	}

	public static async Task TransformAdd(HttpContext context, HttpRequestMessage message)
	{
		if(message.Content == null)
			return;
		
		var jsonPayload = await message.Content.ReadAsStringAsync();
		var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);
		if (jsonObject != null && 
		    !jsonObject.ContainsKey("stock_label_type") && 
		    context.GetRouteData().Values.TryGetValue("productId", out var productId))
		{
			try
			{
				var requestsService = context.RequestServices.GetRequiredService<RequestsService>();
				var productDetail = await requestsService.RequestFromGrocy(message, string.Format(StringConstants.ENDPOINT_Stock_Product_Detail, productId));
				jsonObject.Add("stock_label_type", productDetail.product.default_stock_label_type.ToString());
				message.Content = JsonContent.Create(jsonObject, mediaType: new MediaTypeHeaderValue("application/json"));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
	}
}