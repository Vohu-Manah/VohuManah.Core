using System;
using System.Runtime.Serialization;

namespace Reporting.NETCore
{
	[Serializable]
	public sealed class MissingEndpointException : ReportServerException
	{
		internal MissingEndpointException(string message, Exception innerException)
			: base(message, null, innerException)
		{
		}
	}
}
