using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class HasUserProfileDependenciesException : ReportCatalogException
	{
		public HasUserProfileDependenciesException(string reportName)
			: base(ErrorCode.rsHasUserProfileDependencies, ErrorStrings.rsHasUserProfileDependencies(reportName), null, null)
		{
		}
	}
}
