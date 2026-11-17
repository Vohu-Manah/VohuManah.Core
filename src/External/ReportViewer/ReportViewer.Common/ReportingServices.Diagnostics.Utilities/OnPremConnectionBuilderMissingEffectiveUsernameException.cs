using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OnPremConnectionBuilderMissingEffectiveUsernameException : ReportCatalogException
	{
		public OnPremConnectionBuilderMissingEffectiveUsernameException()
			: base(ErrorCode.rsOnPremConnectionBuilderMissingEffectiveUsername, ErrorStrings.rsOnPremConnectionBuilderMissingEffectiveUsername, null, null)
		{
		}
	}
}
