using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelRootPolicyRequiredException : ReportCatalogException
	{
		public ModelRootPolicyRequiredException()
			: base(ErrorCode.rsModelRootPolicyRequired, ErrorStrings.rsModelRootPolicyRequired, null, null)
		{
		}
	}
}
