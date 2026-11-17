using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ReportingServices.Diagnostics
{
	internal static class ExternalResourceLoader
	{
		internal static readonly int MaxResourceSizeUnlimited = -1;

		public static byte[] GetExternalResource(string resourceUrl, bool impersonate, string surrogateUser, string surrogatePassword, string surrogateDomain, int webTimeout, int maxResourceSizeBytes, ExternalResourceAbortHelper abortHelper, out string mimeType, out bool resourceExceededMaxSize)
		{
			byte[] result;
			mimeType = null;
			resourceExceededMaxSize = false;
			Uri uri = new Uri(resourceUrl);
			int timeoutMs = (webTimeout > 0 && webTimeout < 2147483) ? webTimeout * 1000 : 600000;

			if (uri.IsFile)
			{
				using (Stream fileStream = File.OpenRead(uri.LocalPath))
				{
					result = ((maxResourceSizeBytes != MaxResourceSizeUnlimited) ? StreamSupport.ReadToEndNotUsingLength(fileStream, 1024, maxResourceSizeBytes, out resourceExceededMaxSize) : StreamSupport.ReadToEndNotUsingLength(fileStream, 1024));
				}
			}
			else
			{
				using HttpClientHandler handler = new HttpClientHandler();
				if (surrogateUser != null)
				{
					handler.Credentials = new NetworkCredential(surrogateUser, surrogatePassword, surrogateDomain);
				}
				else if (impersonate)
				{
					handler.Credentials = CredentialCache.DefaultCredentials;
				}
				using HttpClient httpClient = new HttpClient(handler);
				using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
				using CancellationTokenSource cts = new CancellationTokenSource(timeoutMs);
				using HttpResponseMessage webResponse = RequestExternalResource(httpClient, request, abortHelper, cts);
				mimeType = webResponse.Content.Headers.ContentType?.MediaType;
				using (Stream s = webResponse.Content.ReadAsStream(cts.Token))
				{
					result = ((maxResourceSizeBytes != MaxResourceSizeUnlimited) ? StreamSupport.ReadToEndNotUsingLength(s, 1024, maxResourceSizeBytes, out resourceExceededMaxSize) : StreamSupport.ReadToEndNotUsingLength(s, 1024));
				}
			}
			if (uri.IsFile && !resourceExceededMaxSize)
			{
				string text = Path.GetExtension(uri.LocalPath).ToUpperInvariant();
				if (text != null && text.StartsWith(".", StringComparison.Ordinal))
				{
					text = text.Substring(1);
				}
				string mimeTypeByRegistryLookup = GetMimeTypeByRegistryLookup(text);
				if (mimeTypeByRegistryLookup != null)
				{
					mimeType = mimeTypeByRegistryLookup;
				}
			}
			return result;
		}

		public static bool IsValidResourceSize(int maxResourceBytes, byte[] contents)
		{
			if (maxResourceBytes != MaxResourceSizeUnlimited && contents != null)
			{
				return contents.Length <= maxResourceBytes;
			}
			return true;
		}

		// abortHelper polling is folded onto the same timeout CancellationTokenSource:
		// the original code raced a 1s-polling loop against WebRequest.Abort(); HttpClient
		// has no equivalent abort-in-flight primitive, so cancelling the same token that
		// already bounds the request achieves the same effect (fail the in-flight send).
		private static HttpResponseMessage RequestExternalResource(HttpClient httpClient, HttpRequestMessage request, ExternalResourceAbortHelper abortHelper, CancellationTokenSource cts)
		{
			using System.Threading.Timer abortPollTimer = (abortHelper == null) ? null : new System.Threading.Timer(delegate
			{
				if (abortHelper.IsAborted)
				{
					cts.Cancel();
				}
			}, null, 1000, 1000);

			HttpResponseMessage response = httpClient.Send(request, cts.Token);
			response.EnsureSuccessStatusCode();
			return response;
		}

		public static string GetMimeTypeByRegistryLookup(string extension)
		{
			return "image/bmp";
		}
	}
}
