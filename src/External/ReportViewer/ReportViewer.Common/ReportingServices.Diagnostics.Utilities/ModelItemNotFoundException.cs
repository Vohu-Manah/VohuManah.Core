using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelItemNotFoundException : ReportCatalogException
	{
		public ModelItemNotFoundException(string modelPath, string modelItemID)
			: base(ErrorCode.rsModelItemNotFound, ErrorStrings.rsModelItemNotFound(modelPath, modelItemID), null, null)
		{
		}
	}
}
