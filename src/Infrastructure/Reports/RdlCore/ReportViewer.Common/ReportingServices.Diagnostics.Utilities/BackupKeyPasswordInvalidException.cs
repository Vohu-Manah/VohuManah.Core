using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class BackupKeyPasswordInvalidException : ReportCatalogException
	{
		public BackupKeyPasswordInvalidException()
			: base(ErrorCode.rsBackupKeyPasswordInvalid, ErrorStrings.rsBackupKeyPasswordInvalid, null, null)
		{
		}
	}
}
