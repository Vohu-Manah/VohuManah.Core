using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_SpatialTypeComparisonError : Exception
	{
		private const string TypeSerializationID = "type";

		private string m_type;

		internal string Type => m_type;

		internal ReportProcessingException_SpatialTypeComparisonError(string type)
		{
			m_type = type;
		}

	}
}
