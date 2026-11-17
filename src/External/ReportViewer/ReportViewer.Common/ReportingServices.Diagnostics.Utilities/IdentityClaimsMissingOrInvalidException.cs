using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class IdentityClaimsMissingOrInvalidException : ReportCatalogException
	{
		public IdentityClaimsMissingOrInvalidException(string identityClaims)
			: base(ErrorCode.rsIdentityClaimsMissingOrInvalid, ErrorStrings.rsIdentityClaimsMissingOrInvalid((identityClaims == null) ? string.Empty : identityClaims), null, null)
		{
		}
	}
}
