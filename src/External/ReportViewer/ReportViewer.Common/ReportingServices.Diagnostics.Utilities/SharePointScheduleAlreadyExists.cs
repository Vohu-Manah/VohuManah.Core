using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SharePointScheduleAlreadyExists : ReportCatalogException
	{
		public SharePointScheduleAlreadyExists(string name, string path)
			: base(ErrorCode.rsScheduleAlreadyExists, ErrorStrings.rsSharePoitScheduleAlreadyExists(name, path), null, null)
		{
		}
	}
}
