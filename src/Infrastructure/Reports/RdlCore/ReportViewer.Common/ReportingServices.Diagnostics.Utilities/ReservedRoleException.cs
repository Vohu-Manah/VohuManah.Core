using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReservedRoleException : ReportCatalogException
	{
		public ReservedRoleException(string roleName)
			: base(ErrorCode.rsReservedRole, ErrorStrings.rsReservedRole(roleName), null, null)
		{
		}
	}
}
