using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RoleNotFoundException : ReportCatalogException
	{
		public RoleNotFoundException(string roleName)
			: base(ErrorCode.rsRoleNotFound, ErrorStrings.rsRoleNotFound(roleName), null, null)
		{
		}
	}
}
