using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InheritedPolicyException : ReportCatalogException
	{
		public InheritedPolicyException(string itemPath)
			: base(ErrorCode.rsInheritedPolicy, ErrorStrings.rsInheritedPolicy(itemPath), null, null)
		{
		}

		public InheritedPolicyException(string itemPath, string itemID)
			: base(ErrorCode.rsInheritedPolicy, ErrorStrings.rsInheritedPolicyModelItem(itemPath, itemID), null, null)
		{
		}
	}
}
