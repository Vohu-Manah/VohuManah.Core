using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotDeleteRootPolicyException : ReportCatalogException
	{
		public CannotDeleteRootPolicyException()
			: base(ErrorCode.rsCannotDeleteRootPolicy, ErrorStrings.rsCannotDeleteRootPolicy, null, null)
		{
		}
	}
}
