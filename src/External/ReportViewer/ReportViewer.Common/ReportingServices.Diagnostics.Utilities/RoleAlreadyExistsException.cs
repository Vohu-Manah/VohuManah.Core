using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RoleAlreadyExistsException : ReportCatalogException
	{
		public RoleAlreadyExistsException(string roleName)
			: base(ErrorCode.rsRoleAlreadyExists, ErrorStrings.rsRoleAlreadyExists(roleName), null, null)
		{
		}
	}
}
