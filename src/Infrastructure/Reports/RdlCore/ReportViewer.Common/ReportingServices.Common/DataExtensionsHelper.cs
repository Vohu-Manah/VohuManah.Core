using System;
using System.Reflection;

namespace ReportingServices.Common
{
	internal sealed class DataExtensionsHelper
	{
		internal static Type GetDataExtensionConnectionType(string extensionProvider, string getProviderConnectionTypeMethod)
		{
			try
			{
				return (Type)Assembly.Load("ReportingServices.DataExtensions.dll").GetType(extensionProvider).InvokeMember(getProviderConnectionTypeMethod, BindingFlags.Static | BindingFlags.Public, null, null, null);
			}
			catch
			{
				return null;
			}
		}
	}
}
