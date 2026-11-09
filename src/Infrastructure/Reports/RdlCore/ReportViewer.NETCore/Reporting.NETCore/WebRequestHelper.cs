using ReportingServices.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace Reporting.NETCore
{
	internal static class WebRequestHelper
	{
		private const string InfoQuery = "rs:MoreInformation";

		private const string SPUserTokenParam = "rs:TrustedUserToken";

		private const string SPUserNameParam = "rs:TrustedUserName";

		// Replaces the old GetServerUrlAccessObject(...):HttpWebRequest + caller-side GetResponse().
		// HttpWebRequest's per-request Credentials/CookieContainer have no HttpClient equivalent -
		// those move to a per-call HttpClientHandler (matching the pattern already used by
		// ExternalResourceLoader.cs's own WebRequest->HttpClient migration), since credentials here
		// are genuinely per-call (impersonated/forms/bearer), not a fixed shared identity.
		public static HttpResponseMessage ExecuteServerUrlRequest(string url, ICredentials credentials, Cookie formsAuthCookie, IEnumerable<string> headers, IEnumerable<Cookie> cookies, string userName, string bearerToken, byte[] userToken, CancellationToken cancellationToken)
		{
			using HttpClientHandler handler = new HttpClientHandler
			{
				Credentials = credentials,
				CookieContainer = new CookieContainer()
			};
			if (formsAuthCookie != null)
			{
				handler.CookieContainer.Add(formsAuthCookie);
			}
			if (cookies != null)
			{
				foreach (Cookie cooky in cookies)
				{
					handler.CookieContainer.Add(cooky);
				}
			}
			using HttpClient httpClient = new HttpClient(handler);
			using HttpRequestMessage request = CreateRequest(url, headers, bearerToken, userName, userToken);
			return httpClient.Send(request, cancellationToken);
		}

		private static HttpRequestMessage CreateRequest(string url, IEnumerable<string> headers, string bearerToken, string userName, byte[] userToken)
		{
			HttpRequestMessage request;
			if (userToken != null && !string.IsNullOrEmpty(userName))
			{
				string input = Convert.ToBase64String(userToken);
				string content = string.Format(CultureInfo.InvariantCulture, "{0}={1}&{2}={3}", SPUserNameParam, UrlUtil.UrlEncode(userName), SPUserTokenParam, UrlUtil.UrlEncode(input));
				request = new HttpRequestMessage(HttpMethod.Post, url)
				{
					Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded")
				};
			}
			else
			{
				request = new HttpRequestMessage(HttpMethod.Get, url);
			}
			request.Headers.TryAddWithoutValidation("Accept-Language", Thread.CurrentThread.CurrentCulture.Name);
			if (!string.IsNullOrEmpty(bearerToken))
			{
				request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {bearerToken}");
			}
			AddRawHeaders(request, headers);
			return request;
		}

		// `headers` is a flat "Name: Value" string list (see ReportViewerHeaderCollection) - the
		// same format WebHeaderCollection.Add(string) used to parse for the old HttpWebRequest path.
		private static void AddRawHeaders(HttpRequestMessage request, IEnumerable<string> headers)
		{
			if (headers == null)
			{
				return;
			}
			foreach (string header in headers)
			{
				int separatorIndex = header.IndexOf(':');
				if (separatorIndex <= 0)
				{
					continue;
				}
				string name = header.Substring(0, separatorIndex).Trim();
				string value = header.Substring(separatorIndex + 1).Trim();
				request.Headers.TryAddWithoutValidation(name, value);
			}
		}

		// Replaces the old ExceptionFromWebResponse(Exception):ReportServerException, which relied on
		// HttpWebRequest.GetResponse() throwing WebException with a readable .Response on non-2xx
		// status. HttpClient doesn't throw on non-2xx by default, so the caller now passes the
		// HttpResponseMessage explicitly once it has decided the response is an error (non-success
		// status), alongside any transport-level exception actually thrown (cancellation, socket
		// errors). transportException is null when called purely because of a non-success status.
		public static ReportServerException ExceptionFromWebResponse(HttpResponseMessage response, Exception transportException)
		{
			return ReportServerException.FromException(ExceptionFromWebResponseUnwrapped(response, transportException));
		}

		private static Exception ExceptionFromWebResponseUnwrapped(HttpResponseMessage response, Exception transportException)
		{
			if (transportException is OperationCanceledException)
			{
				return new OperationCanceledException();
			}
			if (transportException is IOException ioException && ioException.InnerException is SocketException socketException && socketException.SocketErrorCode == SocketError.Interrupted)
			{
				return new OperationCanceledException();
			}
			if (response != null && !response.IsSuccessStatusCode)
			{
				Exception moreInformationException = TryParseMoreInformationFault(response);
				if (moreInformationException != null)
				{
					return moreInformationException;
				}
			}
			return transportException ?? new HttpRequestException($"Server URL request failed with status {response?.StatusCode}.");
		}

		private static Exception TryParseMoreInformationFault(HttpResponseMessage response)
		{
			try
			{
				using Stream responseStream = response.Content.ReadAsStream();
				XmlDocument xmlDocument = new XmlDocument();
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.CheckCharacters = false;
				using XmlReader reader = XmlReader.Create(responseStream, xmlReaderSettings);
				xmlDocument.Load(reader);
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("rs", "http://www.microsoft.com/sql/reportingservices");
				if (xmlDocument.DocumentElement != null)
				{
					return ReportServerException.FromMoreInformationNode(xmlDocument.DocumentElement.SelectSingleNode(InfoQuery, xmlNamespaceManager));
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
