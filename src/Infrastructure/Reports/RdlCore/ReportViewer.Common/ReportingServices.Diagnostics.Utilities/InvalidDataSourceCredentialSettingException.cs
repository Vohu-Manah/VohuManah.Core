using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidDataSourceCredentialSettingException : ReportCatalogException
	{
		public InvalidDataSourceCredentialSettingException()
			: base(ErrorCode.rsInvalidDataSourceCredentialSetting, ErrorStrings.rsInvalidDataSourceCredentialSetting, null, null)
		{
		}
	}
}
