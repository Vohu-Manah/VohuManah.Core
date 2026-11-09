using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class EmptyRoleException : ReportCatalogException
	{
		public EmptyRoleException()
			: base(ErrorCode.rsEmptyRole, ErrorStrings.rsEmptyRole, null, null)
		{
		}
	}
}
