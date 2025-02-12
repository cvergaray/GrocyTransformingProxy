namespace GrocyTransformingProxy.Helpers;

public static class UriExtensions
{
	public static Uri RelativeToFullPath(this string relativePath)
	{
		var baseUriString = Environment.GetEnvironmentVariable(StringConstants.BaseUrlEnvironmentVariable);
		if (baseUriString == null)
			throw new ArgumentException("Base URI not set");
		var endpointUri = new Uri(new Uri(baseUriString), relativePath);
		return endpointUri;
	}
}