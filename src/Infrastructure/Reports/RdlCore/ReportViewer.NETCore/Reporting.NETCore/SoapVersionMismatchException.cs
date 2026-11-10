using System;
using System.Runtime.Serialization;

namespace Reporting.NETCore
{
	[Serializable]
	public sealed class SoapVersionMismatchException : ReportServerException
	{
		internal SoapVersionMismatchException(string message, Exception innerException)
			: base(message, null, innerException)
		{
		}
	}
}
