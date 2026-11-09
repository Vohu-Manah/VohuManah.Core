using ReportingServices.Common;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_ComparisonError : Exception, IDataComparisonError
	{
		private const string TypeXSerializationID = "typex";

		private const string TypeYSerializationID = "typey";

		private string m_typeX;

		private string m_typeY;

		public string TypeX => m_typeX;

		public string TypeY => m_typeY;

		internal ReportProcessingException_ComparisonError(string typeX, string typeY)
		{
			m_typeX = typeX;
			m_typeY = typeY;
		}

	}
}
