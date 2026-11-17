using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CertificateMissingOrInvalidException : ReportCatalogException
	{
		public CertificateMissingOrInvalidException(string certificateId)
			: base(ErrorCode.rsCertificateMissingOrInvalid, ErrorStrings.rsCertificateMissingOrInvalid(certificateId), null, null)
		{
		}
	}
}
