using System.Net.Http.Headers;
using AspNetCore.Proxy.Builders;
using GrocyTransformingProxy.Helpers;
using GrocyTransformingProxy.Strings;
using Newtonsoft.Json;

namespace GrocyTransformingProxy.Endpoints.Stock;

// Stock/Products
public static class Products
{
	#region {productId}/Add

	public static IProxiesBuilder MapProductAdd(this IProxiesBuilder proxies)
	{
		proxies.Map( IncomingEndpoints.Stock_Product_Add, proxy => proxy.UseHttp(
			(_, args) => string.Format(OutgoingEndpoints.Stock_Product_Add, args[IncomingEndpoints.Stock_Product_Add_Param_ProductId]).RelativeToFullPath().ToString(),
			options => options.WithBeforeSend(TransformAdd)
		));
		return proxies;
	}

	public static async Task TransformAdd(HttpContext context, HttpRequestMessage message)
	{
		if(message.Content == null)
			return;
		
		var jsonPayload = await message.Content.ReadAsStringAsync();
		var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);
		if (jsonObject != null && 
		    !jsonObject.ContainsKey(StringConstants.Stock_Label_Type_Key) && 
		    context.GetRouteData().Values.TryGetValue(IncomingEndpoints.Stock_Product_Add_Param_ProductId, out var productId))
		{
			try
			{
				var requestsService = context.RequestServices.GetRequiredService<RequestService>();
				var productDetail = await requestsService.RequestFromGrocy(message, string.Format(OutgoingEndpoints.Stock_Product_Detail, productId));
				jsonObject.Add(StringConstants.Stock_Label_Type_Key, productDetail.product.default_stock_label_type.ToString());
				message.Content = JsonContent.Create(jsonObject, mediaType: new MediaTypeHeaderValue("application/json"));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
	}

	#endregion
}